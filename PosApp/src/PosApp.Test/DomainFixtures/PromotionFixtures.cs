using Autofac;
using NHibernate.Util;
using PosApp.Domain;
using PosApp.Repositories;

namespace PosApp.Test.DomainFixtures
{
    public class PromotionFixtures
    {
        readonly ILifetimeScope m_scope;

        public PromotionFixtures(ILifetimeScope scope)
        {
            m_scope = scope;
        }

        public void Create(params Promotion[] promotion)
        {
            var promotionRepository = m_scope.Resolve<IPromotionRepository>();
            promotionRepository.Save(promotion);
        }

    }
}