base classes.
1. What they are
SetupEntity
A SetupEntity is reference/configuration/master data.
•	Base chain: SetupEntity -> SetupData -> AuditEntity
•	Defined in:
•	MIDALibraries/MIDACoreDomainModel/DomainModel/SetupEntity.cs:3-5
•	MIDALibraries/MIDACoreDomainModel/DomainModel/SetupData.cs:7-19
SetupData adds:
•	Guid? Guid
•	bool ObsoleteFlag
•	localization support hook (Localized, SetLocalized())
So setup data is intended to be:
•	stable across environments through Guid
•	seedable
•	soft-deletable/obsoletable
•	often used as lookup/match/config tables
Example:
•	Tag : SetupEntity in TestMIDALoBX/SolutionLoB/DomainModel/SharedResourceModule/Tag.auto.cs:15-30
•	mapped to table S_Tag in .../TagConfiguration.auto.cs:15-18
---
SetupRelation
A SetupRelation is still setup/configuration data, but modeled as a relationship row between setup/config entities.
•	Base chain: SetupRelation -> SetupData -> AuditEntity
•	Defined in MIDALibraries/MIDACoreDomainModel/DomainModel/SetupRelation.cs:3-5
It has the same base behavior as SetupEntity:
•	Guid
•	ObsoleteFlag
•	audit fields
•	seeding support
The distinction is semantic/conventional:
•	SetupEntity = setup record
•	SetupRelation = setup association/config row
Example:
•	AssignmentType_CommTemplate : SetupRelation in TestMIDALoBX/SolutionLoB/DomainModel/AssignmentModule/AssignmentType_CommTemplate.auto.cs:19-33
•	mapped to table SR_AssignmentType_CommTemplate in .../AssignmentType_CommTemplateConfiguration.auto.cs:15-18
This example is also marked [MatchTable], so setup relations are often used for rule/matching/config resolution.
---
BusinessEntity
A BusinessEntity is transactional/operational domain data.
•	Base chain: BusinessEntity -> BusinessData -> AuditEntity
•	Defined in:
•	MIDALibraries/MIDACoreDomainModel/DomainModel/BusinessEntity.cs:3-4
•	MIDALibraries/MIDACoreDomainModel/DomainModel/BusinessData.cs:3-4
BusinessData adds no extra fields beyond audit fields.
So business data does not have:
•	Guid
•	ObsoleteFlag
•	built-in setup seeding semantics
Example:
•	Person : BusinessEntity in TestMIDALoBX/SolutionLoB/DomainModel/PersonModule/Person.auto.cs:18-20
•	mapped to table B_Person in .../PersonConfiguration.auto.cs:14-17
---
BusinessRelation
A BusinessRelation is transactional relation data.
•	Base chain: BusinessRelation -> BusinessData -> AuditEntity
•	Defined in MIDALibraries/MIDACoreDomainModel/DomainModel/BusinessRelation.cs:3-4
This is used for:
•	many-to-many with payload
•	temporal relationships
•	business association records
Examples:
•	PersonRelation : BusinessRelation in .../PersonRelation.auto.cs:15-31
•	Assignment_Person : BusinessRelation in .../Assignment_Person.auto.cs:18-34
Mapped example:
•	PersonRelation -> BR_PersonRelation in .../PersonRelationConfiguration.auto.cs:14-17
---
2. The real architectural split
Common base for all four
All four inherit AuditEntity, which provides:
•	Id
•	App_CreateUserId
•	App_UpdateUserId
•	App_CreateDate
•	App_UpdateDate
•	App_Version
•	access profile audit fields
•	validation hooks
See MIDALibraries/MIDACoreDomainModel/DomainModel/AuditEntity.cs:8-29.
So the key split is:
Category	Base	Extra semantics
SetupEntity	SetupData	master/config row
SetupRelation	SetupData	master/config relation row
BusinessEntity	BusinessData	transactional row
BusinessRelation	BusinessData	transactional relation row
---
3. Operational differences
Setup side
SetupData-based types:
•	have Guid
•	have ObsoleteFlag
•	are eligible for seeding
•	support soft delete
•	are used in matching/configuration engines
Evidence:
•	ISetupData defines Guid and ObsoleteFlag in MIDALibraries/MIDACoreDomainModel/Contracts/ISetupData.cs:3-8
•	DelSetupDataService sets ObsoleteFlag = true instead of physical delete in MIDALibraries/MIDACoreServiceLayer/Services/DelSetupDataService.cs:22-25
•	SetupDataFilterBuilder supports filtering by ObsoleteFlag in .../Filters/SetupDataFilterBuilder.cs:10-23
Business side
BusinessData-based types:
•	do not have setup GUID semantics
•	are not part of setup seeding
•	use normal CRUD lifecycle
•	delete is currently physical
Evidence:
•	IBusinessData is only IAuditEntity in MIDALibraries/MIDACoreDomainModel/Contracts/IBusinessData.cs:3-5
•	DelBusinessDataService calls Repository.Delete(repoElement) in .../Services/DelBusinessDataService.cs:22-26
---
4. How seeding works for SetupEntity and SetupRelation
Important: seeding is based on ISetupData / SetupData, not on SetupEntity only.
That means:
•	SetupEntity is seeded
•	SetupRelation is seeded
•	anything inheriting SetupData is included
Seeding entry point
SeedingManager.ApplySeedingAsync():
•	scans EF model for all entity types implementing ISetupData
•	sorts them by FK dependency
•	resolves all ISetupDataSeeding<T> services for each type
•	builds and applies seeds
•	saves using a special seeding audit user
See MIDALibraries/MIDACoreEFPersistence/Configuration/SeedingManager.cs:36-69.
---
How it is triggered
Seeding is not started from Program.cs.
It runs through MigrationsManager when the DbSeeding operation is requested:
•	--DbSeeding
•	-s
See:
•	MIDALibraries/MIDACoreEFPersistence/Configuration/MigrationsManager.cs:44-46
•	MIDALibraries/MIDACoreEFPersistence/Configuration/MigrationsManager.cs:72-76
•	parameter mapping in .../MigrationsManager.cs:91-103
So operationally:
1.	migration manager runs
2.	setup seeding executes
3.	programmability objects sync executes
---
Dependency ordering
Before seeding, entities are ordered by foreign keys using EntitySortingManager.
See:
•	SeedingManager.cs:38-47
•	MIDALibraries/MIDACoreEFPersistence/Seeding/EntitySortingManager.cs:13-31
Why:
•	setup relations usually depend on setup entities
•	relation tables must be seeded after principal setup tables
If a FK points to something not part of setup seeding, it logs a warning and continues:
•	EntitySortingManager.cs:56-59
---
Seeder registration model
Seeders are normal DI services implementing:
•	ISetupDataSeeding<TEntity> in MIDALibraries/MIDACoreEFPersistence/Configuration/ISetupDataSeeding.cs:5-8
Typical seeders are auto-registered with:
•	[AutoRegister(RegistrationLifetime.Scoped, typeof(ISetupDataSeeding<>))]
Examples:
•	RootEntitySeeding.cs:10-12
•	SchemaTypeSeeding.cs:7-9
•	UserRole_PrivilegeSeeding.cs:13-18
So the architecture is plug-in based:
•	each module contributes seeders
•	the central manager discovers and executes them
---
Builder behavior
Each seeder receives ISetupSeedingBuilder<TEntity>.
Main operations:
•	LoadOneAsync<TRelated>()
•	LoadManyAsync<TRelated>()
•	Seed(IEnumerable<TEntity>, Action<TEntity,TEntity> updateFunction)
Defined in SeedingManager.cs:88-98.
This is the core mechanism.
---
Insert/update/delete rules
SetupSeedingBuilder<TEntity> does this:
Insert
If incoming seed Guid is not found in current DB collection:
•	it is added
SeedingManager.cs:122
Update
It updates only rows that:
•	already exist by Guid
•	were last updated by the seeding user (App_UpdateUserId == -1)
See:
•	SeedingManager.cs:123-125
•	seeding user id is -1 in SeedingUser.cs:7-10
This is critical:
•	seeded rows remain maintainable by seeders
•	manually edited rows are protected from automatic overwrite
Delete / obsolete
If a previously seeded row is no longer present in the incoming seed set:
•	and it was last updated by the seeding user
•	and it is not already obsolete
then it is marked:
•	ObsoleteFlag = true
See SeedingManager.cs:146-153.
So setup seeding is:
•	idempotent
•	safe
•	soft-removing
•	GUID-based
Duplicate seed protection
If two seed objects share the same Guid, it throws:
•	InvalidOperationException
See SeedingManager.cs:128-130.
---
5. How SetupRelation seeding specifically works
There is no separate engine for SetupRelation.
It is seeded exactly like any SetupData type.
The usual pattern is:
1.	seed principal setup entities first
2.	load them by Guid
3.	read their runtime DB Id
4.	create relation rows using those Ids
5.	seed relation rows by stable relation Guid
Example
UserRole_PrivilegeSeeding shows the exact pattern:
•	load referenced setup rows:
•	LoadOneAsync<TUserRole>(...)
•	LoadOneAsync<TPrivilege>(...)
•	create relation rows with UserRoleId and PrivilegeId
•	seed them with stable Guid
See Modules/MIDACoreModules/MIDACoreModules/Integrations/AssignmentDocumentWorkflow/Seeding/UserRole_PrivilegeSeeding.cs:23-50.
This is the canonical SetupRelation seeding model.
Another example using LoadManyAsync:
•	SideEffectDataSchemaSeeding.cs:21-48
---
6. How setup data is used at runtime
Setup data is not just lookup data; it also drives matching/configuration resolution.
SetupMatchManager
SetupMatchManager<TMatchEntity> works only on SetupData:
•	where TMatchEntity : SetupData
See MIDALibraries/MIDACoreEFPersistence/SetupMatchManager.cs:12-14.
It:
•	queries setup rows
•	automatically excludes obsolete records
•	supports include graphs
•	supports cache
•	applies matching criteria
•	orders and evaluates match candidates
Important filter:
•	.Where(x => !x.ObsoleteFlag) in SetupMatchManager.cs:189-191
EntityMatchManager
The in-memory matcher for setup entities also excludes obsolete records:
•	entities.Where(e => !e.ObsoleteFlag)
See MIDALibraries/MIDACoreServiceLayer/Matching/EntityMatchManager.cs:19-26.
So operationally:
•	setup tables can act like dynamic rule tables
•	setup relations often power matching and configurability
---
7. Table naming convention in this solution
From the generated EF configurations:
•	SetupEntity -> S_*
•	example: S_Tag
•	SetupRelation -> SR_*
•	example: SR_AssignmentType_CommTemplate
•	BusinessEntity -> B_*
•	example: B_Person
•	BusinessRelation -> BR_*
•	example: BR_PersonRelation
This is convention-driven and visible in the auto-generated configuration files.
---
8. Practical mental model
Use this mental model:
•	SetupEntity = seeded lookup/config/master row
•	SetupRelation = seeded config association / match table row
•	BusinessEntity = live transactional domain record
•	BusinessRelation = live transactional association record
And:
•	setup = stable by Guid, soft-obsoletable, seed-managed
•	business = operational, CRUD-managed, not seed-managed by this framework
---
9. Short conclusion
The most important architectural fact is:
The real split is not entity vs relation.
The real split is SetupData vs BusinessData.
Entity/Relation is mainly a modeling convention.
Setup vs Business drives the actual behavior:
•	seeding
•	soft deletion
•	matching
•	filtering
•	GUID identity strategy
If needed, I can next provide:
1.	a diagram of the inheritance and runtime flow, or
2.	a table listing concrete entities in this solution under each category.
