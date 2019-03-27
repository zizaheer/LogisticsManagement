using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LogisticsManagement_Web.Controllers
{
    public class TariffController : Controller
    {
        private Lms_TariffLogic _tariffLogic;
        private App_CityLogic _cityLogic;
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_VehicleTypeLogic _vehicleTypeLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;

        private readonly LogisticsContext _dbContext;

        public TariffController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _tariffLogic = new Lms_TariffLogic(new EntityFrameworkGenericRepository<Lms_TariffPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            return View(GetTariffData());
        }


        [HttpGet]
        public IActionResult Get()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic tariffData)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(tariffData);
                Lms_TariffPoco[] pocos = JsonConvert.DeserializeObject<Lms_TariffPoco[]>(serializedData);

                _tariffLogic.Add(pocos);
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        [HttpPut]
        public IActionResult Update([FromBody]dynamic tariffData)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(tariffData);
                Lms_TariffPoco[] pocos = JsonConvert.DeserializeObject<Lms_TariffPoco[]>(serializedData);

                _tariffLogic.Update(pocos);
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        [HttpPost]
        public IActionResult Remove([FromBody]dynamic tariffData)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(tariffData);
                Lms_TariffPoco[] pocos = JsonConvert.DeserializeObject<Lms_TariffPoco[]>(serializedData);

                _tariffLogic.Remove(pocos);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_PartialView", GetTariffData());
        }

        private TariffViewModel GetTariffData()
        {
            _cityLogic = new App_CityLogic(new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            _vehicleTypeLogic = new Lms_VehicleTypeLogic(new EntityFrameworkGenericRepository<Lms_VehicleTypePoco>(_dbContext));
            _unitTypeLogic = new Lms_UnitTypeLogic(new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            _weightScaleLogic = new Lms_WeightScaleLogic(new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));

            TariffViewModel tariffViewModel = new TariffViewModel();

            tariffViewModel.Tariffs = _tariffLogic.GetAllList();
            tariffViewModel.Cities = _cityLogic.GetAllList();
            tariffViewModel.DeliveryOptions = _deliveryOptionLogic.GetAllList();
            tariffViewModel.VehicleTypes = _vehicleTypeLogic.GetAllList();
            tariffViewModel.UnitTypes = _unitTypeLogic.GetAllList();
            tariffViewModel.WeightScales = _weightScaleLogic.GetAllList();

            return tariffViewModel;
        }

    }
}