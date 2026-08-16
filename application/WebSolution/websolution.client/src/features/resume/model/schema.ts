import { z } from 'zod';

const optionalText = (maxLength: number) =>
    z
        .string()
        .trim()
        .max(maxLength, `Keep this under ${maxLength} characters.`);

const requiredText = (maxLength: number, label: string) =>
    optionalText(maxLength).min(1, `${label} is required.`);

const optionalDate = z
    .string()
    .transform((value) => value.trim())
    .refine((value) => value === '' || !Number.isNaN(Date.parse(value)), {
        message: 'Enter a valid date.',
    });

const optionalUrl = z
    .string()
    .trim()
    .refine((value) => value === '' || z.url().safeParse(value).success, {
        message: 'Enter a valid URL.',
    });

const requiredUrl = z
    .string()
    .trim()
    .min(1, 'Profile URL is required.')
    .refine((value) => z.url().safeParse(value).success, {
        message: 'Enter a valid URL.',
    });

const optionalEmail = z
    .string()
    .trim()
    .refine((value) => value === '' || z.email().safeParse(value).success, {
        message: 'Enter a valid email address.',
    });

const optionalNumberString = z
    .string()
    .trim()
    .refine((value) => value === '' || !Number.isNaN(Number(value)), {
        message: 'Enter a valid number.',
    });

const itemBaseSchema = z.object({
    clientKey: z.string(),
    id: z.number().int().nonnegative(),
});

const socialMediaLinkSchema = itemBaseSchema.extend({
    displayName: optionalText(100),
    platform: requiredText(100, 'Platform'),
    url: requiredUrl,
});

const awardSchema = itemBaseSchema.extend({
    awardDate: optionalDate,
    awardName: requiredText(200, 'Award name'),
    description: optionalText(1000),
    issuingOrganization: optionalText(200),
});

const hobbySchema = itemBaseSchema.extend({
    description: optionalText(500),
    hobbyName: requiredText(100, 'Hobby'),
});

const languageSchema = itemBaseSchema.extend({
    description: optionalText(500),
    languageName: requiredText(100, 'Language'),
    proficiencyLevel: optionalText(100),
});

const educationSchema = itemBaseSchema.extend({
    city: optionalText(100),
    degree: optionalText(200),
    description: optionalText(1000),
    grade: optionalText(50),
    institution: optionalText(200),
    isCurrent: z.boolean(),
    passingYear: optionalNumberString.refine(
        (value) => value === '' || Number(value) > 1900,
        'Enter a valid passing year.',
    ),
    percentage: optionalNumberString.refine(
        (value) => value === '' || (Number(value) >= 0 && Number(value) <= 100),
        'Use a value between 0 and 100.',
    ),
    state: optionalText(100),
});

const certificationSchema = itemBaseSchema.extend({
    certificationName: requiredText(200, 'Certification name'),
    credentialId: optionalText(100),
    credentialUrl: optionalUrl,
    description: optionalText(1000),
    expirationDate: optionalDate,
    issueDate: optionalDate,
    issuingOrganization: optionalText(200),
});

const skillSchema = itemBaseSchema.extend({
    description: optionalText(500),
    proficiencyLevel: optionalText(100),
    skillName: requiredText(100, 'Skill'),
    yearsOfExperience: optionalNumberString,
});

const workExperienceSchema = itemBaseSchema.extend({
    companyName: optionalText(200),
    description: optionalText(1000),
    endDate: optionalDate,
    isCurrent: z.boolean(),
    jobTitle: optionalText(150),
    location: optionalText(200),
    startDate: optionalDate,
});

const projectSchema = itemBaseSchema.extend({
    description: optionalText(1000),
    endDate: optionalDate,
    isCurrent: z.boolean(),
    projectName: requiredText(200, 'Project name'),
    role: optionalText(150),
    startDate: optionalDate,
    technologiesUsed: optionalText(500),
});

export const resumeFormSchema = z.object({
    certifications: z
        .array(certificationSchema)
        .min(1, 'Add at least one certification entry.'),
    education: z
        .array(educationSchema)
        .min(1, 'Add at least one education entry.'),
    personalInformation: itemBaseSchema.extend({
        address: itemBaseSchema.extend({
            city: optionalText(100),
            country: optionalText(100),
            line1: optionalText(200),
            line2: optionalText(200),
            postalCode: optionalText(20),
            state: optionalText(100),
        }),
        awards: z.array(awardSchema),
        email: optionalEmail,
        hobbies: z.array(hobbySchema),
        languages: z.array(languageSchema),
        phoneNumber: optionalText(30),
        removeAddress: z.boolean(),
        socialMediaLinks: z.array(socialMediaLinkSchema),
        useSameAwardsAsProfile: z.boolean(),
        useSameEmailAsProfile: z.boolean(),
        useSameHobbiesAsProfile: z.boolean(),
        useSameLanguagesAsProfile: z.boolean(),
        useSamePhoneNumberAsProfile: z.boolean(),
        useSameSocialMediaLinksAsProfile: z.boolean(),
    }),
    photo: optionalUrl,
    projects: z.array(projectSchema).min(1, 'Add at least one project entry.'),
    resumeId: z.number().int().nullable(),
    resumeTemplate: requiredText(100, 'Template'),
    skills: z.array(skillSchema).min(1, 'Add at least one skill entry.'),
    summary: optionalText(2000),
    title: requiredText(200, 'Resume title'),
    userId: requiredText(450, 'User'),
    workExperiences: z
        .array(workExperienceSchema)
        .min(1, 'Add at least one work experience entry.'),
});

export type ResumeFormSchema = z.infer<typeof resumeFormSchema>;
