# Small N Stats Discounting Model Selector
Discounting Model Selector is a WPF application that uses R interop libraries (RdotNet) to easily facilitate many complex calculations, including the fitting of multiple models of intertemporal choice as well as a bayesian model comparison method, using a friendly spreadsheet-based approach.  

Features include:
  - Non-linear model fittings (Exponential, Hyperbolic, Quasi-Hyperbolic/b-d, and both Hyperboloid Variants)
  - Discounting Model comparison/competition using Discounting information criterion (BIC) to inform the best/better fitting models (see Franck et al., 2015)
  - ED50 calculations for all models, all parameter fittings, and overall model competition (e.g., best, 2nd best, ...)
  - Identification of best performing model, with corresponding ED50, for similar cross-model discounting indices
  - R-based graphical output in appropriate log space(s) for each model selected (ggplot2)
  - Easily imports common file formats into the application's spreadsheet (.xlsx, .csv)
  - Wide range of model outputs/metrics, saveable in common spreadsheet file formats

### Version
1.0.1.14

### Changelog
 * 1.0.1.4 - Incorporated native icon visuals, fixed link following, corrected grid styling, and better grid management with 10k+ series.
 * 1.0.1.0 - Incorporated recently re-licensed Reogrid control (MIT).  Begin migrate away from manual VM grid managagement and xml write/reads. 
 * 1.0.0.85 - User reviews/feedback: 1) Added new goodness of fit measures (i.e., AIC), 2) optional bounding for models with corresponding user feedback, 3) spreadsheet reading bugs squashed and 4) recents menu improved. 
 * 1.0.0.84 - Bounding settings, name changes and updates for clarity
 * 1.0.0.82 - Upstream changes..., licensing updates, formatting updates, standardized precision, spelling fixes, etc. 
 * 1.0.0.74 - Ugly Cut/Paste bug fixed. Added context menu with transposition options!
 * 1.0.0.73 - Various minor bug fixes, cleanup from private beta, language changes
 * 1.0.0.70 - Expanded output- Model probabilities output directly 
 * 1.0.0.69 - Finally fixed start menu word errors from pre-1.0.0.0
 * 1.0.0.68 - Few UI tweaks related to new combined VM/view
 * 1.0.0.67 - Limited refactor - Merge batched/single calculations to single VM to avoid redundancy
 * 1.0.0.66 - Error handling fix, bar plot tweaks
 * 1.0.0.65 - Citation updates
 * 1.0.0.64 - UI tweaks, all parameters reported as lognormal
 * 1.0.0.63 - UI tweaks, more sanitizing of user input
 * 1.0.0.62 - Format licenses 
 * 1.0.0.61 - Initial licensing agreement prompt added
 * 1.0.0.60 - Overwrite bugs fixed
 * 1.0.0.59 - Interactive visuals for initial install, Saves .svg files natively
 * 1.0.0.58 - Add Help menu for basic troubleshooting and information for submitting logs
 * 1.0.0.57 - New interactive UI for new installers (Color coded for clarity), fixed bug on startup if R not present
 * 1.0.0.56 - Fix for missing dependency (new graphics), formatting for older machines (non-Aero)

### Referenced Works (F/OSS software)
The Small N Stats Discounting Model Selector uses a number of open source projects to work properly:
* R Statistical Package - GPLv2 Licensed. Copyright (C) 2000-16. The R Core Team [Site](https://www.r-project.org/)
* RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved [Github](https://github.com/jmp75/rdotnet)
* SharpVectors: Vector graphics rendering for WPF - New BSD License (BSD 3-Clause). Copyright(c) 2010, SharpVectorGraphics. All rights reserved [Codeplex](http://sharpvectors.codeplex.com/)
* RdotNet/Dynamic Interop - MIT Licensed. Copyright (c) 2015 Jean-Michel Perraud, Copyright (c) 2014 Daniel Collins, CSIRO, Copyright (c) 2013 Kosei, evolvedmicrobe. [Github](https://github.com/jmp75/dynamic-interop-dll)
* Reogrid - MIT Licensed. Copyright(c) 2013-2016 Jing{lujing at unvell.com}, Copyright(c) 2013-2016 unvell.com. [Github](https://github.com/unvell/ReoGrid)

### Referenced Works (R packages/scripts)
The Discounting Model Selector accesses the following R packages to perform statistical methods:
* nls R Package - GPLv2 Licensed. Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro.
* nls R Package - GPLv2 Licensed. Copyright (C) 2000-7. The R Core Team.
* ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.
* gridExtra R Package - GPLv2 Licensed. Copyright (c) 2016, Baptiste Auguie.
* base64enc R Package - GPLv2 Licensed. Copyright (c) 2015, Simon Urbanek
* reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham.
* scales R Package - MIT Licensed. Copyright (c) 2010-2014, Hadley Wickham.
* BDS R Script - GPLv2 Licensed. Copyright (c) 2016, Dr. Chris Franck, Virginia Tech - Department of Statistics.

### Referenced Works (academic works)
The Small N Stats Discounting Model Selector is based on the following academic works:
* Franck, C. T., Koffarnus, M. N., House, L. L. & Bickel W. K. (2015). Accurate characterization of delay discounting: a multiple model approach using approximate Bayesian model selection and a unified discounting measure. Journal of the Experimental Analysis of Behavior, 103, 218-33.

### Acknowledgements and Credits
* Donald A. Hantula, Decision Making Laboratory, Temple University [Site](http://astro.temple.edu/~hantula/)
* Chris Franck, Laboratory for Interdisciplinary Statistical Analysis - Virginia Tech

### Installation
You will need the R open-source statistical package for model fitting/charting to be performed.
Once Discounting Model Selector is installed, it will perform a one-time install the necessary R packages (internet required).
Discounting Model Selector is a ClickOnce application, the program will automatically update as the program is refined.

### Download
All downloads, if/when posted, will be hosted at [Small N Stats](http://www.smallnstats.com/BayesianSelector.html). 

### Development
Want to contribute? Great! Emails or PM's are welcome.

### Todos
 - Completed! Scheduled for public release late May 2016

### License
----
Discounting Model Selector - Copyright 2016, Shawn P. Gilroy. GPL-Version 2
