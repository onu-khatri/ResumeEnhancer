import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect, useMemo, useRef } from 'react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/auth-context';
import { mapResumeResponseToForm } from '@/features/resume/api/resume-service';
import type { ResumeBuilderFieldArrays } from '@/features/resume/builder/builder-types';
import { useResumeDraft } from '@/features/resume/hooks/use-resume-draft';
import { useResumeQuery } from '@/features/resume/hooks/use-resume-query';
import { useResumeSubmit } from '@/features/resume/hooks/use-resume-submit';
import { resumeFormSchema } from '@/features/resume/model/schema';
import { useResumeDraftStore } from '@/features/resume/state/resume-draft-store';
import {
  createEmptyResumeForm,
  type ResumeFormValues,
} from '@/features/resume/model/types';

export function useResumeBuilderController() {
  const navigate = useNavigate();
  const { session } = useAuth();
  const resumeId = session?.resumeId ?? null;
  const resumeQuery = useResumeQuery(resumeId);
  const submitResume = useResumeSubmit();
  const didHydrateForm = useRef(false);
  const storedDraft = useResumeDraftStore((state) => state.draft);

  const form = useForm<ResumeFormValues>({
    defaultValues: createEmptyResumeForm(session?.userId ?? ''),
    resolver: zodResolver(resumeFormSchema),
  });

  const matchingDraft = useMemo(() => {
    return storedDraft && storedDraft.resumeId === resumeId ? storedDraft : null;
  }, [resumeId, storedDraft]);

  const canHydrate =
    resumeId === null ||
    matchingDraft !== null ||
    resumeQuery.data !== undefined ||
    !resumeQuery.isPending;

  const values = useWatch({ control: form.control }) as ResumeFormValues;
  const { clearDraft, lastSavedAt } = useResumeDraft(values, resumeId, canHydrate);

  const arrays: ResumeBuilderFieldArrays = {
    awards: useFieldArray({
      control: form.control,
      name: 'personalInformation.awards',
    }),
    certifications: useFieldArray({ control: form.control, name: 'certifications' }),
    education: useFieldArray({ control: form.control, name: 'education' }),
    hobbies: useFieldArray({
      control: form.control,
      name: 'personalInformation.hobbies',
    }),
    languages: useFieldArray({
      control: form.control,
      name: 'personalInformation.languages',
    }),
    projects: useFieldArray({ control: form.control, name: 'projects' }),
    skills: useFieldArray({ control: form.control, name: 'skills' }),
    socialLinks: useFieldArray({
      control: form.control,
      name: 'personalInformation.socialMediaLinks',
    }),
    workExperiences: useFieldArray({
      control: form.control,
      name: 'workExperiences',
    }),
  };

  useEffect(() => {
    if (didHydrateForm.current) {
      return;
    }

    if (matchingDraft) {
      form.reset(matchingDraft.values);
      didHydrateForm.current = true;
      return;
    }

    if (resumeQuery.data) {
      form.reset(mapResumeResponseToForm(resumeQuery.data));
      didHydrateForm.current = true;
      return;
    }

    if (!resumeQuery.isPending) {
      form.reset(createEmptyResumeForm(session?.userId ?? ''));
      didHydrateForm.current = true;
    }
  }, [form, matchingDraft, resumeQuery.data, resumeQuery.isPending, session?.userId]);

  const saveResume = form.handleSubmit(async (nextValues) => {
    const response = await submitResume.mutateAsync(nextValues);
    form.reset(mapResumeResponseToForm(response));
    clearDraft();
    navigate('/app/resume/preview', {
      replace: true,
      state: { savedAt: new Date().toISOString() },
    });
  });

  const retryResumeLoad = () => {
    void resumeQuery.refetch();
  };

  return {
    arrays,
    canHydrate,
    clearDraft,
    form,
    lastSavedAt,
    matchingDraft,
    resumeQuery,
    retryResumeLoad,
    saveResume,
    submitResume,
    values,
  };
}
