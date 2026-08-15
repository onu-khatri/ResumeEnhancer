using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL;

public static class AppDbContextResumeModuleExtensions
{
    public static DbSet<Resume> Resumes(this AppDbContext dbContext) => dbContext.Set<Resume>();

    public static DbSet<PersonalInformation> PersonalInformation(this AppDbContext dbContext) =>
        dbContext.Set<PersonalInformation>();

    public static DbSet<Address> Addresses(this AppDbContext dbContext) => dbContext.Set<Address>();

    public static DbSet<Award> Awards(this AppDbContext dbContext) => dbContext.Set<Award>();

    public static DbSet<Language> Languages(this AppDbContext dbContext) => dbContext.Set<Language>();

    public static DbSet<Hobby> Hobbies(this AppDbContext dbContext) => dbContext.Set<Hobby>();

    public static DbSet<SocialMediaLink> SocialMediaLinks(this AppDbContext dbContext) =>
        dbContext.Set<SocialMediaLink>();

    public static DbSet<Education> Education(this AppDbContext dbContext) => dbContext.Set<Education>();

    public static DbSet<Certification> Certifications(this AppDbContext dbContext) =>
        dbContext.Set<Certification>();

    public static DbSet<Skill> Skills(this AppDbContext dbContext) => dbContext.Set<Skill>();

    public static DbSet<WorkExperience> WorkExperiences(this AppDbContext dbContext) =>
        dbContext.Set<WorkExperience>();

    public static DbSet<Project> Projects(this AppDbContext dbContext) => dbContext.Set<Project>();

    public static DbSet<ResumeSectionSetup> ResumeSectionSetups(this AppDbContext dbContext) =>
        dbContext.Set<ResumeSectionSetup>();
}

