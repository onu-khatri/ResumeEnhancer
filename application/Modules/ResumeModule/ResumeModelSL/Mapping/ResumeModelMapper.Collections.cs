using DomainLibrary.DomainModel;
using ResumeModuleAM.Requests;

namespace ResumeModuleSL.Handlers;

internal static partial class ResumeModelMapper
{
    private static void SyncCollection<TElement, TRequest>(
        ICollection<TElement> target,
        IList<TRequest>? incoming,
        Func<TRequest, TElement> createElement,
        Action<TElement, TRequest> updateElement,
        Action<AuditEntity> remove,
        string collectionName)
        where TElement : AuditEntity
    {
        if (incoming is null)
        {
            return;
        }

        var existingById = target
            .Where(element => element.Id > 0)
            .ToDictionary(element => element.Id);
        var incomingIds = incoming
            .Select(GetRequestId)
            .Where(id => id > 0)
            .ToHashSet();

        foreach (var request in incoming)
        {
            var id = GetRequestId(request);

            if (id == 0)
            {
                target.Add(createElement(request));
                continue;
            }

            if (!existingById.TryGetValue(id, out var existingElement))
            {
                throw new InvalidOperationException(
                    $"{collectionName} item '{id}' does not belong to the current resume.");
            }

            updateElement(existingElement, request);
        }

        foreach (var removedElement in target
                     .Where(element => element.Id > 0 && !incomingIds.Contains(element.Id))
                     .ToArray())
        {
            target.Remove(removedElement);
            remove(removedElement);
        }
    }

    private static void AddCreatedChildren<TElement, TRequest>(
        ICollection<TElement> target,
        IList<TRequest> incoming,
        Func<TRequest, TElement> createElement)
        where TElement : AuditEntity
    {
        foreach (var request in incoming)
        {
            target.Add(createElement(request));
        }
    }

    private static int GetRequestId<TRequest>(TRequest request) =>
        request switch
        {
            EducationRequest education => education.Id,
            CertificationRequest certification => certification.Id,
            SkillRequest skill => skill.Id,
            WorkExperienceRequest workExperience => workExperience.Id,
            ProjectRequest project => project.Id,
            AwardRequest award => award.Id,
            LanguageRequest language => language.Id,
            HobbyRequest hobby => hobby.Id,
            SocialMediaLinkRequest socialMediaLink => socialMediaLink.Id,
            _ => throw new InvalidOperationException(
                $"Request type '{typeof(TRequest).Name}' does not expose an Id.")
        };

    private static void RemoveOwned(AuditEntity? element, Action<AuditEntity> remove)
    {
        if (element is not null)
        {
            remove(element);
        }
    }
}
