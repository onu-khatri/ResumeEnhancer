/*
 Entities to store the resume data for the user
1. Resume: Title, Summary, Photo, ResumeTemplate, UserId - FK
2. PeronalInformation
    Address: - id, City, State, Country, ZipCode, street address - FK
    Email , useSameAsProfile
    PhoneNumber, useSameAsProfile
    Awards, useSameAsProfile - FK
    Languages, useSameAsProfile - FK
    hobbies, useSameAsProfile - Fk    
    socialMediaLinks, useSameAsProfile - FK
3. Education
    PassingYear, Degree, Institution, City, State, Description, percentage, Grade, isCurrent
r. Certifications
    CertificationName, IssuingOrganization, IssueDate, ExpirationDate, CredentialID, CredentialURL, Description
5. Skills
    SkillName, ProficiencyLevel, YearsOfExperience, Description
6. WorkExperience
    JobTitle, CompanyName, StartDate, EndDate, Location, Description, isCurrent
7. Projects
    ProjectName, Role, StartDate, EndDate, Description, TechnologiesUsed, isCurrent
4. Setup Table: List of Education, Certifications, Skills, Languages, WorkExperience, Projects, Awards, Hobbies, SocialMediaLinks
  
 */