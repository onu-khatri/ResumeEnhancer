export interface ResumeListItemResponse {
  app_CreateDate: string;
  app_UpdateDate: string | null;
  certificationCount: number;
  educationCount: number;
  id: number;
  photo: string | null;
  projectCount: number;
  resumeTemplate: string | null;
  skillCount: number;
  summary: string | null;
  title: string;
  userId: string;
  workExperienceCount: number;
}

export interface ResumeSearchResponse {
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  items: ResumeListItemResponse[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ResumeSearchRequest {
  hasPhoto?: boolean;
  pageNumber: number;
  pageSize: number;
  resumeTemplate?: string | null;
  searchText?: string | null;
  sortBy?: number;
  sortDirection?: number;
  userId?: string | null;
}

export interface ResumeDeleteResponse {
  deletedCount: number;
  deletedIds: number[];
  forbiddenIds: number[];
  hasFailures: boolean;
  notFoundIds: number[];
  requestedIds: number[];
}
