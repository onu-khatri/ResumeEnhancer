export interface ResumeListItemFormBase {
  clientKey: string;
  id: number;
}

export interface AddressFormValues extends ResumeListItemFormBase {
  city: string;
  country: string;
  line1: string;
  line2: string;
  postalCode: string;
  state: string;
}

export interface AwardFormValues extends ResumeListItemFormBase {
  awardDate: string;
  awardName: string;
  description: string;
  issuingOrganization: string;
}

export interface CertificationFormValues extends ResumeListItemFormBase {
  certificationName: string;
  credentialId: string;
  credentialUrl: string;
  description: string;
  expirationDate: string;
  issueDate: string;
  issuingOrganization: string;
}

export interface EducationFormValues extends ResumeListItemFormBase {
  city: string;
  degree: string;
  description: string;
  grade: string;
  institution: string;
  isCurrent: boolean;
  passingYear: string;
  percentage: string;
  state: string;
}

export interface HobbyFormValues extends ResumeListItemFormBase {
  description: string;
  hobbyName: string;
}

export interface LanguageFormValues extends ResumeListItemFormBase {
  description: string;
  languageName: string;
  proficiencyLevel: string;
}

export interface ProjectFormValues extends ResumeListItemFormBase {
  description: string;
  endDate: string;
  isCurrent: boolean;
  projectName: string;
  role: string;
  startDate: string;
  technologiesUsed: string;
}

export interface SkillFormValues extends ResumeListItemFormBase {
  description: string;
  proficiencyLevel: string;
  skillName: string;
  yearsOfExperience: string;
}

export interface SocialMediaLinkFormValues extends ResumeListItemFormBase {
  displayName: string;
  platform: string;
  url: string;
}

export interface WorkExperienceFormValues extends ResumeListItemFormBase {
  companyName: string;
  description: string;
  endDate: string;
  isCurrent: boolean;
  jobTitle: string;
  location: string;
  startDate: string;
}

export interface PersonalInformationFormValues extends ResumeListItemFormBase {
  address: AddressFormValues;
  awards: AwardFormValues[];
  email: string;
  hobbies: HobbyFormValues[];
  languages: LanguageFormValues[];
  phoneNumber: string;
  removeAddress: boolean;
  socialMediaLinks: SocialMediaLinkFormValues[];
  useSameAwardsAsProfile: boolean;
  useSameEmailAsProfile: boolean;
  useSameHobbiesAsProfile: boolean;
  useSameLanguagesAsProfile: boolean;
  useSamePhoneNumberAsProfile: boolean;
  useSameSocialMediaLinksAsProfile: boolean;
}

export interface ResumeFormValues {
  certifications: CertificationFormValues[];
  education: EducationFormValues[];
  personalInformation: PersonalInformationFormValues;
  photo: string;
  projects: ProjectFormValues[];
  resumeId: number | null;
  resumeTemplate: string;
  skills: SkillFormValues[];
  summary: string;
  title: string;
  userId: string;
  workExperiences: WorkExperienceFormValues[];
}
