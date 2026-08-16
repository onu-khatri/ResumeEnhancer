import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/auth-context';
import { useResumeDelete } from '@/features/resume/hooks/use-resume-delete';
import { useResumeSearch } from '@/features/resume/hooks/use-resume-search';

const PAGE_SIZE = 12;

export function useResumeDashboard() {
    const navigate = useNavigate();
    const { session, updateActiveResumeId } = useAuth();
    const [pageNumber, setPageNumber] = useState(1);
    const [searchText, setSearchText] = useState('');

    const request = useMemo(
        () => ({
            pageNumber,
            pageSize: PAGE_SIZE,
            searchText: searchText.trim() || null,
            sortBy: 2,
            sortDirection: 1,
            userId: session?.userId ?? null,
        }),
        [pageNumber, searchText, session?.userId],
    );

    const resumeSearch = useResumeSearch(request);
    const deleteResume = useResumeDelete();

    const selectResume = (resumeId: number, target: 'builder' | 'preview') => {
        updateActiveResumeId(resumeId);
        navigate(`/app/resume/${target}`);
    };

    const createResume = () => {
        updateActiveResumeId(null);
        navigate('/app/resume/builder');
    };

    return {
        createResume,
        deleteResume,
        pageNumber,
        resumeSearch,
        searchText,
        selectResume,
        setPageNumber,
        setSearchText,
    };
}
