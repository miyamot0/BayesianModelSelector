# Small N Stats Bayesian Model Selector
Bayesian Model Selector is a WPF application that uses R interop libraries (RdotNet) to easily facilitate many complex calculations, including the fitting of multiple models of intertemporal choice as well as a bayesian model comparison method, using a friendly spreadsheet-based approach.  
Features include:
  - Non-linear model fittings (Exponential, Hyperbolic, Quasi-Hyperbolic/b-d, and both Hyperboloid Variants)
  - Bayesian Model comparison/competition using Bayesian information criterion (BIC) to inform the best/better fitting models (see Franck et al., 2015)
  - ED50 calculations for all models, all parameter fittings, and overall model competition (e.g., best, 2nd best, ...)
  - Identification of best performing model, with corresponding ED50, for similar cross-model discounting indices
  - R-based graphical output in appropriate log space(s) for each model selected (ggplot2)
  - Easily imports common file formats into the application's spreadsheet (.xlsx, .csv)
  - Wide range of model outputs/metrics, saveable in common spreadsheet file formats

### Version
1.0.0.36

### Referenced Works (F/OSS software)
The Small N Stats Demand Calculator uses a number of open source projects to work properly:
* R Statistical Package - GPL v2 Licensed. Copyright (C) 2000-16. The R Core Team [Site](https://www.r-project.org/)
* RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved [Github](https://github.com/jmp75/rdotnet)
* RdotNet/Dynamic Interop - MIT Licensed. Copyright (c) 2015 Jean-Michel Perraud, Copyright (c) 2014 Daniel Collins, CSIRO, Copyright (c) 2013 Kosei, evolvedmicrobe. [Github](https://github.com/jmp75/dynamic-interop-dll)
* ClosedXML - MIT Licensed. Copyright (c) 2010 Manuel De Leon. [Codeplex](http://closedxml.codeplex.com/SourceControl/latest)

### Referenced Works (R packages/scripts)
* nls R Package - GPLv2 Licensed. Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro.
* nls R Package - GPLv2 Licensed. Copyright (C) 2000-7. The R Core Team.
* ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.
* gridExtra R Package - GPLv2+ Licensed. Copyright (c) 2016, Baptiste Auguie.
* reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham.
* BDS R Script - GPLv2 Licensed. Copyright (c) 2016, Dr. Chris Franck, Virginia Tech - Department of Statistics.

### Referenced Works (academic works)
The Small N Stats Bayesian Model Selector is based on the following academic works:
* Franck, C. T., Koffarnus, M. N., House, L. L. & Bickel W. K. (2015). Accurate characterization of delay discounting: a multiple model approach using approximate Bayesian model selection and a unified discounting measure. Journal of the Experimental Analysis of Behavior, 103, 218-33.

### Acknowledgements and Credits
* Donald A. Hantula, Decision Making Laboratory, Temple University [Site](http://astro.temple.edu/~hantula/)
* Chris Franck, Laboratory for Interdisciplinary Statistical Analysis - Virginia Tech

### Installation
You will need the R open-source statistical package for model fitting/charting to be performed.
Once Bayesian Model Selector is installed, it will perform a one-time install the necessary R packages (internet required).
Bayesian Model Selector is a ClickOnce application, the program will automatically update as the program is refined.

### Development
Want to contribute? Great! Emails or PM's are welcome.

### Todos
 - Completed! Scheduled for public release late May 2016

### License
----
GPL-Version 2
