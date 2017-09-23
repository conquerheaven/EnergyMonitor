using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Entity
{
    public class EnergyAllTypeEntity
    {
        public BuildingDetailInfo building { get; set; }
        public RoomAndBuilding roomAndBuilding { get; set; }
        public AreaAndSchool area { get; set; }
        public AreaAndSchool school { get; set; }
        public AnalogMeasurePoint point { get; set; }
        public IList<EnergyEntity> powerElecReals { get; set; }
        public IList<EnergyEntity> powerWaterReals { get; set; }
        public IList<EnergyEntity> powerGasReals { get; set; }

        public IList<ChartStatisEntity> valByHoursElec { get; set; }
        public IList<ChartStatisEntity> valByHoursWater { get; set; }
        public IList<ChartStatisEntity> valByHoursGas { get; set; }

        public IList<ChartStatisEntity> valByDaysElec { get; set; }
        public IList<ChartStatisEntity> valByDaysWater { get; set; }
        public IList<ChartStatisEntity> valByDaysGas { get; set; }

        public IList<ChartStatisEntity> valByMonthsElec { get; set; }
        public IList<ChartStatisEntity> valByMonthsWater { get; set; }
        public IList<ChartStatisEntity> valByMonthsGas { get; set; }
    }
}
