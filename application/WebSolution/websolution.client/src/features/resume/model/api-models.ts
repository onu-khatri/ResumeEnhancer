export interface ResumeDetailResponse {
  app_CreateDate: string;
  app_UpdateDate: string | null;
  app_Version: number[];
  certifications: CertificationApiModel[];
  education: EducationApiModel[];
  id: number;
  personalInformation: PersonalInformationApiModel | null;
  photo: string | null;
  projects: ProjectApiModel[];
  resumeTemplate: string | null;
  skills: SkillApiModel[];
  summary: string | null;
  title: string;
  userId: string;
  workExperiences: WorkExperienceApiModel[];
}

export interface AddressApiModel {
  city: string | null;
  country: string | null;
  id: number;
  line1: string | null;
  line2: string | null;
  postalCode: string | null;
  state: string | null;
}

export interface AwardApiModel {
  awardDate: string | null;
  awardName: string;
  description: string | null;
  id: number;
  issuingOrganization: string | null;
}

export interface CertificationApiModel {
  certificationName: string;
  credentialId: string | null;
  credentialUrl: string | null;
  description: string | null;
  expirationDate: string | null;
  id: number;
  issueDate: string | null;
  issuingOrganization: string | null;
}

export interface EducationApiModel {
  city: string | null;
  degree: string | null;
  description: string | null;
  grade: string | null;
  id: number;
  institution: string | null;
  isCurrent: boolean;
  passingYear: number | null;
  percentage: number | null;
  state: string | null;
}

export interface HobbyApiModel {
  description: string | null;
  hobbyName: string;
  id: number;
}

export interface LanguageApiModel {
  description: string | null;
  id: number;
  languageName: string;
  proficiencyLevel: string | null;
}

export interface ProjectApiModel {
  description: string | null;
  endDate: string | null;
  id: number;
  isCurrent: boolean;
  projectName: string;
  role: string | null;
  startDate: string | null;
  technologiesUsed: string | null;
}

export interface SkillApiModel {
  description: string | null;
  id: number;
  proficiencyLevel: string | null;
  skillName: string;
  yearsOfExperience: number | null;
}

export interface SocialMediaLinkApiModel {
  displayName: string | null;
  id: number;
  platform: string;
  url: string;
}

export interface WorkExperienceApiModel {
  companyName: string | null;
  description: string | null;
  endDate: string | null;
  id: number;
  isCurrent: boolean;
  jobTitle: string | null;
  location: string | null;
  startDate: string | null;
}

export interface PersonalInformationApiModel {
  address: AddressApiModel | null;
  awards: AwardApiModel[];
  email: string | null;
  hobbies: HobbyApiModel[];
  id: number;
  languages: LanguageApiModel[];
  phoneNumber: string | null;
  socialMediaLinks: SocialMediaLinkApiModel[];
  useSameAwardsAsProfile: boolean;
  useSameEmailAsProfile: boolean;
  useSameHobbiesAsProfile: boolean;
  useSameLanguagesAsProfile: boolean;
  useSamePhoneNumberAsProfile: boolean;
  useSameSocialMediaLinksAsProfile: boolean;
}
