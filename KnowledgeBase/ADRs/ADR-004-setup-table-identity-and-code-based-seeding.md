---
title: Setup Table Identity And Code-Based Seeding
status: accepted
date: 2026-08-16
---

# What

This ADR defines how setup-table identity should work in ResumeEnhancer.

The rule is:

1. Setup tables still use `int` primary keys for relational foreign keys.
2. The application must not treat seeded setup `Id` values as business meaning.
3. Setup seeds should define stable `Code` and `Guid` values, but should not hard-code `Id`.
4. When application logic needs a setup-table `Id`, it must read setup data, filter by `Code`, and then use the resolved `Id` only for FK assignment or relational work.

# Why

Hard-coding seeded setup IDs creates a hidden assumption that the database and the code will always evolve together in a fixed order.

That becomes risky when:

- an administrator inserts setup rows directly into the table
- a migration or data import changes insert order
- data is restored or synchronized from another environment
- new setup rows are introduced before old assumptions are revisited

`Code` is the stable business identity.
`Id` is the relational identity.

The application should depend on the stable one for meaning and the relational one for foreign keys.

# When

Use this ADR whenever:

- seeding setup tables
- resolving a setup-table row from business logic
- assigning a setup FK such as `RenderTypeId` or `AddressTypeId`
- validating a request that refers to a setup value

# Where

This ADR applies especially to:

- `*.PL/Seeding`
- `*.SL` handlers and mapping code that resolve setup values
- `*.DM` entities that store setup foreign keys

# Who

This ADR is for:

- backend developers
- reviewers
- architects
- junior engineers wiring setup-table FKs

If someone asks:

- "Can I seed setup rows with fixed IDs?"
- "Can I assume Billing is always address type 1?"
- "How do I get the FK ID for a setup row?"

this ADR is the default answer.

# Problem And Constraints

Setup rows need two identities:

- a database primary key for foreign keys
- a stable business key that the application can safely reason about

The application must keep using `int` FKs in the database model, but it must stop assuming that the integer itself carries business meaning.

# Decision Drivers

- Data safety across environments
- Admin flexibility
- Lower hidden coupling between code and insert order
- Clear separation between business identity and relational identity

# Decision

## Rule 1: `Code` is the business identity

The application should use setup-table `Code` as the stable semantic identifier.

Good example:

```csharp
var renderType = renderTypes.SingleOrDefault(item =>
    string.Equals(item.Code, request.RenderTypeCode, StringComparison.OrdinalIgnoreCase));
```

## Rule 2: `Id` is the relational identity only

The application may still assign:

```csharp
entity.RenderTypeId = renderType.Id;
```

but only after the setup row has been resolved by `Code`.

## Rule 3: Do not seed setup rows with explicit `Id`

Avoid this:

```csharp
new TemplateRenderTypeSetup
{
    Id = 1,
    Code = "Pdf"
}
```

Prefer this:

```csharp
new TemplateRenderTypeSetup
{
    Code = nameof(TemplateRenderType.Pdf),
    Guid = ...
}
```

## Rule 4: Resolve setup IDs through setup-data access, not through code constants

If the application needs a setup FK value, the flow should be:

1. read setup rows from the owning module
2. filter by `Code`
3. take the resolved `Id`
4. assign that `Id` to the FK property

Example:

```csharp
var addressTypes = await _setupDataRepository.ListUserAddressTypesAsync(cancellationToken);
var billingAddressType = addressTypes.Single(item => item.Code == nameof(UserAddressType.Billing));

userAddress.AddressTypeId = billingAddressType.Id;
```

# How A Junior Engineer Should Decide

Use this checklist:

1. Do I need the setup row's meaning?
   Use `Code`.

2. Do I need the setup row's FK value?
   First resolve the row by `Code`, then use `Id`.

3. Am I about to write `Id = (int)SomeEnum.Value` in setup seeds?
   Stop. Seed `Code` and `Guid` instead.

4. Am I about to compare a setup FK directly to a magic number or enum-cast number?
   Stop. Resolve the setup row by `Code`.

# Considered Options

## Option 1: Fixed seeded IDs as business constants

### Benefits

- simpler short-term code

### Costs

- fragile across environments
- unsafe with admin-managed data
- hidden coupling between code and insert order

## Option 2: Code-based business identity with database-generated IDs

### Benefits

- safer data evolution
- clearer intent
- preserves relational FK model

### Costs

- requires explicit setup-row resolution before FK assignment

This option is accepted.

# Consequences

## Positive

- Setup logic is safer when data changes outside the original seed order.
- Admin-added rows do not break hidden code assumptions.
- The code makes business identity explicit.

## Negative

- Setup-aware handlers need one more lookup step before assigning FKs.

# Follow-Up Actions

1. Remove fixed setup IDs from seed data.
2. Replace business logic that assumes enum-cast IDs with code-based lookup.
3. Keep FK properties as `int`, but never treat those ints as semantic constants.

# Related ADRs And Evidence

- [ADR-003-setup-entities-over-persisted-enums.md](./ADR-003-setup-entities-over-persisted-enums.md)
- [ADR-005-cached-setup-data-repositories.md](./ADR-005-cached-setup-data-repositories.md)
- [TemplateModuleSeeder.cs](/D:/RND/ResumeEnhancer/application/Modules/TemplateModule/TemplateModulePL/Seeding/TemplateModuleSeeder.cs)
- [UserHandlers.cs](/D:/RND/ResumeEnhancer/application/Modules/ProfilingModule/ProfilingModelSL/Handlers/UserHandlers.cs)
