﻿using Landis.Core;
using Landis.Library.Succession;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Extension.Insects;

namespace Landis.Extension.LandUse.LandCover
{
     /**
      * A class demonstrating LU+ connectivity to succession modules
      * and Insect extension. Can directly call static Defoliation 
      * methods from Insect library
      **/
    class InsectDefoliation 
        : IChange
    {
        public const string TypeName = "InsectDefoliation";
        private Planting.SpeciesList speciesToPlant;

        string IChange.Type { get { return TypeName; } }

        public InsectDefoliation(Planting.SpeciesList plants)
        {
            speciesToPlant = plants;
            CohortDefoliation.Compute = InsectDefoliate;
        }

        public static double InsectDefoliate(ICohort cohort, ActiveSite active, int siteBiomass)
        {
            //Do LU+ specific things?
            //Tentative pseudo-code:
            /* Wherever parameters on LandUses are stored, we should have insect defoliation data
             * Use the active site to access this data
             * Apply the species names and percentages to the ICohort - This happens in tree removal
             * Maybe define a new type of IChange for insect defoliation?
             * Return relevant data
             */
            return 0;
        }

        public void ApplyTo(ActiveSite site)
        {
            // Hopefully we are doing something helpful by using the interface this way
            // I think the idea was that qualitatively different changes to landscapes
            // all subscribe to this interface. Maybe we can leverage the way cohort harvest
            // is parsed from the landuse.txt for RemoveTrees.cs
        }
    }
}
