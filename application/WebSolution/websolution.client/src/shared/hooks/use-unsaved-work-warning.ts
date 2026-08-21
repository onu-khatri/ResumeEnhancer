import { useEffect } from 'react';

export function useUnsavedWorkWarning(hasUnsavedWork: boolean) {
    useEffect(() => {
        if (!hasUnsavedWork) return;

        const onBeforeUnload = (event: BeforeUnloadEvent) => {
            event.preventDefault();
            event.returnValue = '';
        };

        window.addEventListener('beforeunload', onBeforeUnload);
        return () => window.removeEventListener('beforeunload', onBeforeUnload);
    }, [hasUnsavedWork]);
}
