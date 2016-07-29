using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PosApp.Domain;
using PosApp.Dtos.Responses;
using PosApp.Services;

namespace PosApp.Controllers
{
    public class PromotionController:ApiController
    {
        readonly PromotionService m_promotionService;

        public PromotionController(PromotionService promotionService)
        {
            m_promotionService = promotionService;
        }

        [HttpPost]
        public HttpResponseMessage AddPromotions(string addtype, string[] addBarcodes)
        {
            try
            {
                m_promotionService.CreatePromotionsForType(addtype,addBarcodes);
                return Request.CreateResponse(HttpStatusCode.Created,new MessageDto {Message = "Add Promotions success"});
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,new MessageDto {Message = "Add Promotions error" });
            }
        }

        [HttpGet]
        public HttpResponseMessage ListPromotionsForType(string type)
        {
            IList<Promotion> promotions = m_promotionService.GetAllPromotionsForType(type);
            return Request.CreateResponse(HttpStatusCode.OK, promotions);
        }

        [HttpDelete]
        public HttpResponseMessage DeletePromotions(string type, string[] deleteBarcodes)
        {
            m_promotionService.DeletePromotionsForType(type,deleteBarcodes);
            return Request.CreateResponse(HttpStatusCode.OK,new MessageDto {Message = "Delete success"});
        }
    }
}