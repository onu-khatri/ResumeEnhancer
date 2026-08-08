# ResumeModuleDM Project

This project contains the Resume module domain model.

## Entity Categories

Resume module entities inherit the shared domain categories from `DomainLibrary.DomainModel`.

| Entity | Category | Table |
| --- | --- | --- |
| `Resume` | `BusinessEntity` | `resume.B_Resume` |
| `ResumeSectionSetup` | `SetupEntity` | `resume.S_ResumeSectionSetup` |
| `PersonalInformation` | `BusinessRelation` | `resume.BR_PersonalInformation` |
| `Address` | `BusinessRelation` | `resume.BR_Address` |
| `Award` | `BusinessRelation` | `resume.BR_Award` |
| `Certification` | `BusinessRelation` | `resume.BR_Certification` |
| `Education` | `BusinessRelation` | `resume.BR_Education` |
| `Hobby` | `BusinessRelation` | `resume.BR_Hobby` |
| `Language` | `BusinessRelation` | `resume.BR_Language` |
| `Project` | `BusinessRelation` | `resume.BR_Project` |
| `Skill` | `BusinessRelation` | `resume.BR_Skill` |
| `SocialMediaLink` | `BusinessRelation` | `resume.BR_SocialMediaLink` |
| `WorkExperience` | `BusinessRelation` | `resume.BR_WorkExperience` |

Child tables with foreign keys are modeled as business relation tables.

## Rules

- Keep entity classes persistence-light.
- Put EF Core configuration in `ResumeModulePL`.
- Use `SetupEntity` only for seedable setup/master data.
- Use `BusinessRelation` for FK child or relationship rows.
