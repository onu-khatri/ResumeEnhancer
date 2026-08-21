import { LinkIcon, MapPinIcon, PhoneIcon } from '@heroicons/react/24/outline';

import type { ResumeDetailResponse } from '@/features/resume/model/types';
import { PreviewSection } from '@/features/resume/preview/components/preview-section';

export function PreviewSidebar({ resume }: { resume: ResumeDetailResponse }) {
    return (
        <div className="space-y-8">
            <PreviewSection title="Contact">
                <ContactList resume={resume} />
            </PreviewSection>
            <PreviewSection title="Skills">
                <SkillList resume={resume} />
            </PreviewSection>
            <PreviewSection title="Links">
                <SocialLinks resume={resume} />
            </PreviewSection>
        </div>
    );
}

function ContactList({ resume }: { resume: ResumeDetailResponse }) {
    const items = [
        resume.personalInformation?.email
            ? {
                  icon: <LinkIcon className="h-4 w-4" />,
                  label: resume.personalInformation.email,
              }
            : null,
        resume.personalInformation?.phoneNumber
            ? {
                  icon: <PhoneIcon className="h-4 w-4" />,
                  label: resume.personalInformation.phoneNumber,
              }
            : null,
        resume.personalInformation?.address?.city
            ? {
                  icon: <MapPinIcon className="h-4 w-4" />,
                  label: [
                      resume.personalInformation.address.line1,
                      resume.personalInformation.address.city,
                      resume.personalInformation.address.state,
                      resume.personalInformation.address.country,
                  ]
                      .filter(Boolean)
                      .join(', '),
              }
            : null,
    ].filter(Boolean) as Array<{ icon: React.ReactNode; label: string }>;

    if (items.length === 0) {
        return (
            <p className="text-sm text-slate-600 dark:text-slate-400">
                No contact details available.
            </p>
        );
    }

    return (
        <div className="space-y-3">
            {items.map((item) => (
                <div
                    key={item.label}
                    className="flex items-start gap-3 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-800 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-200"
                >
                    <div className="mt-0.5 text-teal-700 dark:text-teal-300">
                        {item.icon}
                    </div>
                    <span>{item.label}</span>
                </div>
            ))}
        </div>
    );
}

function SkillList({ resume }: { resume: ResumeDetailResponse }) {
    return (
        <div className="flex flex-wrap gap-2">
            {resume.skills.length ? (
                resume.skills.map((skill) => (
                    <span
                        key={skill.id}
                        className="rounded-full border border-teal-200 bg-teal-50 px-3 py-2 text-sm font-medium text-teal-950 dark:border-teal-400/20 dark:bg-teal-400/10 dark:text-teal-200"
                    >
                        {skill.skillName}
                    </span>
                ))
            ) : (
                <p className="text-sm text-slate-600 dark:text-slate-400">
                    No skills added yet.
                </p>
            )}
        </div>
    );
}

function SocialLinks({ resume }: { resume: ResumeDetailResponse }) {
    if (!resume.personalInformation?.socialMediaLinks.length) {
        return (
            <p className="text-sm text-slate-600 dark:text-slate-400">
                No social links available.
            </p>
        );
    }

    return (
        <div className="space-y-3">
            {resume.personalInformation.socialMediaLinks.map((link) => (
                <a
                    aria-label={`${link.platform} ${link.displayName ?? 'Open'}`}
                    key={link.id}
                    className="flex items-center justify-between rounded-2xl border border-slate-300 px-4 py-3 text-sm transition hover:border-teal-300 hover:bg-teal-50 dark:border-slate-800 dark:hover:border-teal-400/30 dark:hover:bg-teal-400/10"
                    href={link.url}
                    rel="noreferrer"
                    target="_blank"
                >
                    <span className="font-medium text-slate-900 dark:text-white">
                        {link.platform}
                    </span>
                    <span className="text-slate-600 dark:text-slate-400">
                        {link.displayName ?? 'Open'}
                    </span>
                </a>
            ))}
        </div>
    );
}
