# Small N Stats Bayesian Model Selector
Bayesian Model Selector is a WPF application that uses R interop libraries (RdotNet) to easily facilitate many complex calculations, including the fitting of multiple models of intertemporal choice as well as a bayesian model comparison method.  
Features include:
  - Non-linear model fittings (Exponential, Hyperbolic, Quasi-Hyperbolic/b-d, and both Hyperboloid Variants)
  - Bayesian Model comparison/competition using BIC
  - R-based graphical output in appropriate log space(s) (ggplot2)
  - Easily imports common file formats into the application's spreadsheet (.xlsx, .csv)
  - Wide range of model outputs/metrics, saveable in common spreadsheet file formats

### Version
1.0.0.36

### Referenced Works (F/OSS software)
The Bayesian Model Selector uses a number of open source projects to work properly:
* R Statistical Package - GPL v2 Licensed. Copyright (C) 2000-16. The R Core Team
* RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved [Github](https://github.com/jmp75/rdotnet)
* nls R Package - GPLv2 Licensed. Copyright (C) 1999-1999 Saikat DebRoy, Douglas M. Bates, Jose C. Pinheiro.
* nls R Package - GPLv2 Licensed. Copyright (C) 2000-7. The R Core Team.
* ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.
* gridExtra R Package - GPLv2+ Licensed. Copyright (c) 2016, Baptiste Auguie.
* reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham.
* ClosedXML - MIT Licensed. Copyright (c) 2010 Manuel De Leon.
* BDS R Script - GPLv2 Licensed. Copyright (c) 2016, Dr. Chris Franck, Virginia Tech - Department of Statistics.

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
