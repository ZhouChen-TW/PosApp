using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;
using PosApp.Domain;

namespace PosApp.Repositories
{
    public class PromotionRepository:IPromotionRepository
    {
        readonly ISession m_session;

        public PromotionRepository(ISession session)
        {
            m_session = session;
        }

        public IList<Promotion> GetByType(string type)
        {
            return m_session.Query<Promotion>().Where(p => p.Type.Equals(type)).ToList();
        }

        public IList<Promotion> GetByBarcode(string[] barcodes)
        {
            return m_session.Query<Promotion>().Where(p => barcodes.Contains(p.Barcode)).ToList();
        }

        public int CountByType(string type)
        {
            return m_session.Query<Promotion>().Count(p => p.Type.Equals(type));
        }

        public string[] GetAllTypes()
        {
            return m_session.Query<Promotion>().Select(p => p.Type).Distinct().ToArray();
        }

        public void Save(IList<Promotion> promotions)
        {
            promotions.ForEach(p=>m_session.Save(p));
            m_session.Flush();
        }

        public void Delete(IList<Promotion> promotions)
        {
            promotions.ForEach(d=>m_session.Delete(d));
            m_session.Flush();
        }
    }
}