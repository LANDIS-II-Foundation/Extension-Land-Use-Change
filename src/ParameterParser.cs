// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.Harvest;
using System.Collections.Generic;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// A parser that reads the extension's input and output parameters from
    /// a text file.
    /// </summary>
    public class ParameterParser
        : BasicParameterParser<Parameters>
    {
        // Singleton for all the land uses that have no land cover changes
        private static LandCover.IChange noLandCoverChange = new LandCover.NoChange();

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return Main.ExtensionName;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ParameterParser(ISpeciesDataset speciesDataset)
            : base(speciesDataset, false)
            // The "false" above --> keywords are disabled for cohort selectors
        {
        }

        //---------------------------------------------------------------------

        protected override Parameters Parse()
        {
            ReadLandisDataVar();

            Parameters parameters = new Parameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<string> inputMaps = new InputVar<string>("InputMaps");
            ReadVar(inputMaps);
            parameters.InputMaps = inputMaps.Value;

            ReadLandUses();
            return parameters;
        }

        //---------------------------------------------------------------------

        protected void ReadLandUses()
        {
            InputVar<string> name = new InputVar<string>("LandUse");
            InputVar<ushort> mapCode = new InputVar<ushort>("MapCode");
            InputVar<bool> allowHarvest = new InputVar<bool>("AllowHarvest?");
            InputVar<string> landCoverChangeType = new InputVar<string>("LandCoverChange");

            Dictionary<string, int> nameLineNumbers = new Dictionary<string, int>();
            Dictionary<ushort, int> mapCodeLineNumbers = new Dictionary<ushort, int>();

            while (!AtEndOfInput)
            {
                int nameLineNum = LineNumber;
                ReadVar(name);
                int lineNumber;
                if (nameLineNumbers.TryGetValue(name.Value.Actual, out lineNumber))
                    throw new InputValueException(name.Value.String,
                                                  "The land use \"{0}\" was previously used on line {1}",
                                                  name.Value.Actual, lineNumber);
                else
                {
                    nameLineNumbers[name.Value.Actual] = nameLineNum;
                }

                int mapCodeLineNum = LineNumber;
                ReadVar(mapCode);
                if (mapCodeLineNumbers.TryGetValue(mapCode.Value.Actual, out lineNumber))
                    throw new InputValueException(mapCode.Value.String,
                                                  "The map code \"{0}\" was previously used on line {1}",
                                                  mapCode.Value.Actual, lineNumber);
                else
                    mapCodeLineNumbers[mapCode.Value.Actual] = mapCodeLineNum;

                ReadVar(allowHarvest);

                ReadVar(landCoverChangeType);
                LandCover.IChange landCoverChange = null;
                if (landCoverChangeType.Value.Actual == LandCover.NoChange.TypeName)
                    landCoverChange = noLandCoverChange;
                else if (landCoverChangeType.Value.Actual == LandCover.RemoveTrees.TypeName)
                {
                    ICohortSelector selector = ReadSpeciesAndCohorts("LandUse");
                    landCoverChange = new LandCover.RemoveTrees(selector);
                }
                else
                    throw new InputValueException(landCoverChangeType.Value.String,
                                                  "\"{0}\" is not a type of land cover change",
                                                  landCoverChangeType.Value.Actual);

                LandUse landUse = new LandUse(name.Value.Actual,
                                              mapCode.Value.Actual,
                                              allowHarvest.Value.Actual,
                                              landCoverChange);
                LandUseRegistry.Register(landUse);
            }
        }
    }
}