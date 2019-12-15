using ITelecenter.Models;
using ITelecenter.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ITelecenterRestAPI.Controllers
{
    [RoutePrefix("api/Numbers")]
    public class NumberSearchController : ApiController
    {
        [Route("GetRandomNumbers")]
        [HttpPost]
        public getRandomNumbersResponse GetRandomNumbers(getRandomNumbersRequest request) => NumberSearchService.GetRandomNumbers(request);

        [Route("GetPackages")]
        [HttpPost]
        public GetPackagesResponse GetPackages(ServiceRequest request) => NumberSearchService.GetPackages(request);

        [Route("GetCountries")]
        [HttpPost]
        public GetCountryResponse GetCountries(ServiceRequest request) => NumberSearchService.GetCountries(request);

        [Route("GetStatesByCountry")]
        [HttpPost]
        public GetStateResponse GetStatesByCountry(GetStateRequest request) => NumberSearchService.GetStatesByCountry(request);


        [Route("CreateVisitorInformation")]
        [HttpPut]
        public CreateVisitorInformationResponse CreateVisitorInformation(CreateVisitorInformationRequest request) => NumberSearchService.CreateVisitorInformation(request);
        

    }
}
