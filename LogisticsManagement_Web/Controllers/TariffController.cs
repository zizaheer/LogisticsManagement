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


        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            return PartialView("_PartialView", GetTariffData());
        }


        [HttpPost]
        public IActionResult AddOrUpdate([FromBody]dynamic tariffData)
        {
            var result = false;
            try
            {
                var serializedData = JsonConvert.SerializeObject(tariffData);
                Lms_TariffPoco[] pocos = JsonConvert.DeserializeObject<Lms_TariffPoco[]>(serializedData);

                pocos.FirstOrDefault().CreateDate = DateTime.Now;
                pocos.FirstOrDefault().CreatedBy = 1;
                if (pocos.FirstOrDefault().Id > 0)
                {
                    _tariffLogic.Update(pocos);
                }
                else
                {
                    _tariffLogic.Add(pocos);
                }

                var ffff = _tariffLogic.GetAllList();

                result = true;
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Remove([FromBody]dynamic tariffData)
        {
            bool result = false;
            try
            {
                var serializedData = JsonConvert.SerializeObject(tariffData);
                Lms_TariffPoco[] pocos = JsonConvert.DeserializeObject<Lms_TariffPoco[]>(serializedData);

                _tariffLogic.Remove(pocos);
                result = true;
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        private TariffViewModel GetTariffData()
        {
            _tariffLogic = new Lms_TariffLogic(new EntityFrameworkGenericRepository<Lms_TariffPoco>(_dbContext));
            _cityLogic = new App_CityLogic(new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            _vehicleTypeLogic = new Lms_VehicleTypeLogic(new EntityFrameworkGenericRepository<Lms_VehicleTypePoco>(_dbContext));
            _unitTypeLogic = new Lms_UnitTypeLogic(new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            _weightScaleLogic = new Lms_WeightScaleLogic(new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));

            TariffViewModel tariffViewModel = new TariffViewModel();

            tariffViewModel.Tariffs = _tariffLogic.GetAllList().ToList();
            tariffViewModel.Cities = _cityLogic.GetAllList();
            tariffViewModel.DeliveryOptions = _deliveryOptionLogic.GetAllList();
            tariffViewModel.VehicleTypes = _vehicleTypeLogic.GetAllList();
            tariffViewModel.UnitTypes = _unitTypeLogic.GetAllList();
            tariffViewModel.WeightScales = _weightScaleLogic.GetAllList();

            return tariffViewModel;
        }

    }
}