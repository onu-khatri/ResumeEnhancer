import type { createApiClient } from '@/shared/api/api-client';
import type {
    AddressApiModel,
    AwardApiModel,
    CertificationApiModel,
    EducationApiModel,
    HobbyApiModel,
    LanguageApiModel,
    ProjectApiModel,
    ResumeDetailResponse,
    ResumeDeleteResponse,
    ResumeFormValues,
    ResumeSearchResponse,
    SkillApiModel,
    SocialMediaLinkApiModel,
    WorkExperienceApiModel,
} from '@/features/resume/model/types';

const resumeApiBasePath = '/resumes';

function toNullableString(value: string) {
    const trimmedValue = value.trim();
    return trimmedValue.length > 0 ? trimmedValue : null;
}

function toNullableNumber(value: string) {
    const trimmedValue = value.trim();
    return trimmedValue.length > 0 ? Number(trimmedValue) : null;
}

function mapApiItem<TInput extends { id: number }, TOutput>(
    items: TInput[],
    mapper: (item: TInput) => TOutput,
) {
    return items.map((item) => ({
        ...mapper(item),
        id: item.id > 0 ? item.id : 0,
    }));
}

export function createResumeService(
    apiClient: ReturnType<typeof createApiClient>,
) {
    return {
        createResume: (values: ResumeFormValues) =>
            apiClient.post<ResumeDetailResponse>(
                resumeApiBasePath,
                mapFormToCreateRequest(values),
            ),
        deleteResume: (resumeId: number) =>
            apiClient.delete<ResumeDeleteResponse>(
                `${resumeApiBasePath}/${resumeId}`,
            ),
        deleteResumes: (resumeIds: number[]) =>
            apiClient.post<ResumeDeleteResponse>(
                `${resumeApiBasePath}/delete`,
                { resumeIds },
            ),
        getResume: (resumeId: number) =>
            apiClient.get<ResumeDetailResponse>(
                `${resumeApiBasePath}/${resumeId}`,
            ),
        searchResumes: (request: {
            hasPhoto?: boolean;
            pageNumber: number;
            pageSize: number;
            resumeTemplate?: string | null;
            searchText?: string | null;
            sortBy?: number;
            sortDirection?: number;
            userId?: string | null;
        }) =>
            apiClient.post<ResumeSearchResponse>(
                `${resumeApiBasePath}/search`,
                request,
            ),
        updateResume: (resumeId: number, values: ResumeFormValues) =>
            apiClient.put<ResumeDetailResponse>(
                `${resumeApiBasePath}/${resumeId}`,
                mapFormToUpdateRequest(values),
            ),
    };
}

export function mapResumeResponseToForm(
    resume: ResumeDetailResponse,
): ResumeFormValues {
    const personalInformation = resume.personalInformation;

    return {
        certifications: resume.certifications.map(mapCertificationFromApi),
        education: resume.education.map(mapEducationFromApi),
        personalInformation: {
            address: mapAddressFromApi(personalInformation?.address ?? null),
            awards: (personalInformation?.awards ?? []).map(mapAwardFromApi),
            clientKey: crypto.randomUUID(),
            email: personalInformation?.email ?? '',
            hobbies: (personalInformation?.hobbies ?? []).map(mapHobbyFromApi),
            id: personalInformation?.id ?? 0,
            languages: (personalInformation?.languages ?? []).map(
                mapLanguageFromApi,
            ),
            phoneNumber: personalInformation?.phoneNumber ?? '',
            removeAddress: false,
            socialMediaLinks: (personalInformation?.socialMediaLinks ?? []).map(
                mapSocialLinkFromApi,
            ),
            useSameAwardsAsProfile:
                personalInformation?.useSameAwardsAsProfile ?? false,
            useSameEmailAsProfile:
                personalInformation?.useSameEmailAsProfile ?? false,
            useSameHobbiesAsProfile:
                personalInformation?.useSameHobbiesAsProfile ?? false,
            useSameLanguagesAsProfile:
                personalInformation?.useSameLanguagesAsProfile ?? false,
            useSamePhoneNumberAsProfile:
                personalInformation?.useSamePhoneNumberAsProfile ?? false,
            useSameSocialMediaLinksAsProfile:
                personalInformation?.useSameSocialMediaLinksAsProfile ?? false,
        },
        photo: resume.photo ?? '',
        projects: resume.projects.map(mapProjectFromApi),
        resumeId: resume.id,
        resumeTemplate: resume.resumeTemplate ?? 'executive-clean',
        skills: resume.skills.map(mapSkillFromApi),
        summary: resume.summary ?? '',
        title: resume.title,
        userId: resume.userId,
        workExperiences: resume.workExperiences.map(mapWorkExperienceFromApi),
    };
}

function mapAddressFromApi(address: AddressApiModel | null) {
    return {
        city: address?.city ?? '',
        clientKey: crypto.randomUUID(),
        country: address?.country ?? '',
        id: address?.id ?? 0,
        line1: address?.line1 ?? '',
        line2: address?.line2 ?? '',
        postalCode: address?.postalCode ?? '',
        state: address?.state ?? '',
    };
}

function mapAwardFromApi(award: AwardApiModel) {
    return {
        awardDate: award.awardDate?.slice(0, 10) ?? '',
        awardName: award.awardName,
        clientKey: crypto.randomUUID(),
        description: award.description ?? '',
        id: award.id,
        issuingOrganization: award.issuingOrganization ?? '',
    };
}

function mapCertificationFromApi(certification: CertificationApiModel) {
    return {
        certificationName: certification.certificationName,
        clientKey: crypto.randomUUID(),
        credentialId: certification.credentialId ?? '',
        credentialUrl: certification.credentialUrl ?? '',
        description: certification.description ?? '',
        expirationDate: certification.expirationDate?.slice(0, 10) ?? '',
        id: certification.id,
        issueDate: certification.issueDate?.slice(0, 10) ?? '',
        issuingOrganization: certification.issuingOrganization ?? '',
    };
}

function mapEducationFromApi(education: EducationApiModel) {
    return {
        city: education.city ?? '',
        clientKey: crypto.randomUUID(),
        degree: education.degree ?? '',
        description: education.description ?? '',
        grade: education.grade ?? '',
        id: education.id,
        institution: education.institution ?? '',
        isCurrent: education.isCurrent,
        passingYear: education.passingYear?.toString() ?? '',
        percentage: education.percentage?.toString() ?? '',
        state: education.state ?? '',
    };
}

function mapHobbyFromApi(hobby: HobbyApiModel) {
    return {
        clientKey: crypto.randomUUID(),
        description: hobby.description ?? '',
        hobbyName: hobby.hobbyName,
        id: hobby.id,
    };
}

function mapLanguageFromApi(language: LanguageApiModel) {
    return {
        clientKey: crypto.randomUUID(),
        description: language.description ?? '',
        id: language.id,
        languageName: language.languageName,
        proficiencyLevel: language.proficiencyLevel ?? '',
    };
}

function mapProjectFromApi(project: ProjectApiModel) {
    return {
        clientKey: crypto.randomUUID(),
        description: project.description ?? '',
        endDate: project.endDate?.slice(0, 10) ?? '',
        id: project.id,
        isCurrent: project.isCurrent,
        projectName: project.projectName,
        role: project.role ?? '',
        startDate: project.startDate?.slice(0, 10) ?? '',
        technologiesUsed: project.technologiesUsed ?? '',
    };
}

function mapSkillFromApi(skill: SkillApiModel) {
    return {
        clientKey: crypto.randomUUID(),
        description: skill.description ?? '',
        id: skill.id,
        proficiencyLevel: skill.proficiencyLevel ?? '',
        skillName: skill.skillName,
        yearsOfExperience: skill.yearsOfExperience?.toString() ?? '',
    };
}

function mapSocialLinkFromApi(link: SocialMediaLinkApiModel) {
    return {
        clientKey: crypto.randomUUID(),
        displayName: link.displayName ?? '',
        id: link.id,
        platform: link.platform,
        url: link.url,
    };
}

function mapWorkExperienceFromApi(workExperience: WorkExperienceApiModel) {
    return {
        clientKey: crypto.randomUUID(),
        companyName: workExperience.companyName ?? '',
        description: workExperience.description ?? '',
        endDate: workExperience.endDate?.slice(0, 10) ?? '',
        id: workExperience.id,
        isCurrent: workExperience.isCurrent,
        jobTitle: workExperience.jobTitle ?? '',
        location: workExperience.location ?? '',
        startDate: workExperience.startDate?.slice(0, 10) ?? '',
    };
}

function mapFormToCreateRequest(values: ResumeFormValues) {
    return {
        certifications: mapApiItem(values.certifications, (item) => ({
            certificationName: item.certificationName.trim(),
            credentialId: toNullableString(item.credentialId),
            credentialUrl: toNullableString(item.credentialUrl),
            description: toNullableString(item.description),
            expirationDate: toNullableString(item.expirationDate),
            issueDate: toNullableString(item.issueDate),
            issuingOrganization: toNullableString(item.issuingOrganization),
        })),
        education: mapApiItem(values.education, (item) => ({
            city: toNullableString(item.city),
            degree: toNullableString(item.degree),
            description: toNullableString(item.description),
            grade: toNullableString(item.grade),
            institution: toNullableString(item.institution),
            isCurrent: item.isCurrent,
            passingYear: toNullableNumber(item.passingYear),
            percentage: toNullableNumber(item.percentage),
            state: toNullableString(item.state),
        })),
        personalInformation: mapPersonalInformation(values),
        photo: toNullableString(values.photo),
        projects: mapApiItem(values.projects, (item) => ({
            description: toNullableString(item.description),
            endDate: item.isCurrent ? null : toNullableString(item.endDate),
            isCurrent: item.isCurrent,
            projectName: item.projectName.trim(),
            role: toNullableString(item.role),
            startDate: toNullableString(item.startDate),
            technologiesUsed: toNullableString(item.technologiesUsed),
        })),
        resumeTemplate: toNullableString(values.resumeTemplate),
        skills: mapApiItem(values.skills, (item) => ({
            description: toNullableString(item.description),
            proficiencyLevel: toNullableString(item.proficiencyLevel),
            skillName: item.skillName.trim(),
            yearsOfExperience: toNullableNumber(item.yearsOfExperience),
        })),
        summary: toNullableString(values.summary),
        title: values.title.trim(),
        userId: values.userId,
        workExperiences: mapApiItem(values.workExperiences, (item) => ({
            companyName: toNullableString(item.companyName),
            description: toNullableString(item.description),
            endDate: item.isCurrent ? null : toNullableString(item.endDate),
            isCurrent: item.isCurrent,
            jobTitle: toNullableString(item.jobTitle),
            location: toNullableString(item.location),
            startDate: toNullableString(item.startDate),
        })),
    };
}

function mapFormToUpdateRequest(values: ResumeFormValues) {
    return {
        ...mapFormToCreateRequest(values),
        removePersonalInformation: false,
    };
}

function mapPersonalInformation(values: ResumeFormValues) {
    const personalInformation = values.personalInformation;

    return {
        address: personalInformation.removeAddress
            ? null
            : {
                  city: toNullableString(personalInformation.address.city),
                  country: toNullableString(
                      personalInformation.address.country,
                  ),
                  id:
                      personalInformation.address.id > 0
                          ? personalInformation.address.id
                          : 0,
                  line1: toNullableString(personalInformation.address.line1),
                  line2: toNullableString(personalInformation.address.line2),
                  postalCode: toNullableString(
                      personalInformation.address.postalCode,
                  ),
                  state: toNullableString(personalInformation.address.state),
              },
        awards: mapApiItem(personalInformation.awards, (item) => ({
            awardDate: toNullableString(item.awardDate),
            awardName: item.awardName.trim(),
            description: toNullableString(item.description),
            issuingOrganization: toNullableString(item.issuingOrganization),
        })),
        email: toNullableString(personalInformation.email),
        hobbies: mapApiItem(personalInformation.hobbies, (item) => ({
            description: toNullableString(item.description),
            hobbyName: item.hobbyName.trim(),
        })),
        id: personalInformation.id > 0 ? personalInformation.id : 0,
        languages: mapApiItem(personalInformation.languages, (item) => ({
            description: toNullableString(item.description),
            languageName: item.languageName.trim(),
            proficiencyLevel: toNullableString(item.proficiencyLevel),
        })),
        phoneNumber: toNullableString(personalInformation.phoneNumber),
        removeAddress: personalInformation.removeAddress,
        socialMediaLinks: mapApiItem(
            personalInformation.socialMediaLinks,
            (item) => ({
                displayName: toNullableString(item.displayName),
                platform: item.platform.trim(),
                url: item.url.trim(),
            }),
        ),
        useSameAwardsAsProfile: personalInformation.useSameAwardsAsProfile,
        useSameEmailAsProfile: personalInformation.useSameEmailAsProfile,
        useSameHobbiesAsProfile: personalInformation.useSameHobbiesAsProfile,
        useSameLanguagesAsProfile:
            personalInformation.useSameLanguagesAsProfile,
        useSamePhoneNumberAsProfile:
            personalInformation.useSamePhoneNumberAsProfile,
        useSameSocialMediaLinksAsProfile:
            personalInformation.useSameSocialMediaLinksAsProfile,
    };
}
