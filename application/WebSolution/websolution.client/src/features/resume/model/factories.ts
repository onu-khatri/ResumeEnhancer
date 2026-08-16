import {
    type AddressFormValues,
    type AwardFormValues,
    type CertificationFormValues,
    type EducationFormValues,
    type HobbyFormValues,
    type LanguageFormValues,
    type ProjectFormValues,
    type ResumeFormValues,
    type SkillFormValues,
    type SocialMediaLinkFormValues,
    type WorkExperienceFormValues,
} from '@/features/resume/model/form-values';
import { resumeTemplateOptions } from '@/features/resume/model/template-options';

export function createClientKey() {
    return (
        globalThis.crypto?.randomUUID?.() ??
        `key-${Math.random().toString(36).slice(2)}`
    );
}

export function createAddress(): AddressFormValues {
    return {
        city: '',
        clientKey: createClientKey(),
        country: '',
        id: 0,
        line1: '',
        line2: '',
        postalCode: '',
        state: '',
    };
}

export function createAward(): AwardFormValues {
    return {
        awardDate: '',
        awardName: '',
        clientKey: createClientKey(),
        description: '',
        id: 0,
        issuingOrganization: '',
    };
}

export function createCertification(): CertificationFormValues {
    return {
        certificationName: '',
        clientKey: createClientKey(),
        credentialId: '',
        credentialUrl: '',
        description: '',
        expirationDate: '',
        id: 0,
        issueDate: '',
        issuingOrganization: '',
    };
}

export function createEducation(): EducationFormValues {
    return {
        city: '',
        clientKey: createClientKey(),
        degree: '',
        description: '',
        grade: '',
        id: 0,
        institution: '',
        isCurrent: false,
        passingYear: '',
        percentage: '',
        state: '',
    };
}

export function createHobby(): HobbyFormValues {
    return {
        clientKey: createClientKey(),
        description: '',
        hobbyName: '',
        id: 0,
    };
}

export function createLanguage(): LanguageFormValues {
    return {
        clientKey: createClientKey(),
        description: '',
        id: 0,
        languageName: '',
        proficiencyLevel: '',
    };
}

export function createProject(): ProjectFormValues {
    return {
        clientKey: createClientKey(),
        description: '',
        endDate: '',
        id: 0,
        isCurrent: false,
        projectName: '',
        role: '',
        startDate: '',
        technologiesUsed: '',
    };
}

export function createSkill(): SkillFormValues {
    return {
        clientKey: createClientKey(),
        description: '',
        id: 0,
        proficiencyLevel: '',
        skillName: '',
        yearsOfExperience: '',
    };
}

export function createSocialMediaLink(): SocialMediaLinkFormValues {
    return {
        clientKey: createClientKey(),
        displayName: '',
        id: 0,
        platform: '',
        url: '',
    };
}

export function createWorkExperience(): WorkExperienceFormValues {
    return {
        clientKey: createClientKey(),
        companyName: '',
        description: '',
        endDate: '',
        id: 0,
        isCurrent: false,
        jobTitle: '',
        location: '',
        startDate: '',
    };
}

export function createEmptyResumeForm(userId: string): ResumeFormValues {
    return {
        certifications: [createCertification()],
        education: [createEducation()],
        personalInformation: {
            address: createAddress(),
            awards: [],
            clientKey: createClientKey(),
            email: '',
            hobbies: [],
            id: 0,
            languages: [],
            phoneNumber: '',
            removeAddress: false,
            socialMediaLinks: [createSocialMediaLink()],
            useSameAwardsAsProfile: false,
            useSameEmailAsProfile: false,
            useSameHobbiesAsProfile: false,
            useSameLanguagesAsProfile: false,
            useSamePhoneNumberAsProfile: false,
            useSameSocialMediaLinksAsProfile: false,
        },
        photo: '',
        projects: [createProject()],
        resumeId: null,
        resumeTemplate: resumeTemplateOptions[0].value,
        skills: [createSkill()],
        summary: '',
        title: '',
        userId,
        workExperiences: [createWorkExperience()],
    };
}
