﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController
    {
        private readonly IQualificationSettingService _qualificationService;
        private readonly IGuiVisibilitySettingService _guiVisibilityService;

        public SettingsController(
            ILogFactory logFactory,
            IQualificationSettingService qualificationService,
            IGuiVisibilitySettingService guiVisibilityService) : base(logFactory)
        {
            _qualificationService = qualificationService;
            _guiVisibilityService = guiVisibilityService;
        }

        /// <summary>Liefert alle konfigurierten Qualifications.</summary>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(List<QualificationSettingViewModel>), StatusCodes.Status200OK)]
        public IActionResult Qualifications()
        {
            return Execute(() =>
            {
                return _qualificationService.GetAll();
            });
        }

        /// <summary>Liefert ein ViewModel mit Informationen zur Darstellung der Verfügbarkeiten.</summary>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(GuiVisibilitySettingViewModel), StatusCodes.Status200OK)]
        public IActionResult GuiVisibility()
        {
            return Execute(() =>
            {
                return _guiVisibilityService.GetGuiVisibility();
            });
        }
    }
}