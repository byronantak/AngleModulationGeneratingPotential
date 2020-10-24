# Angle Modulation Generating Potential Experiment

## Tech Stack

This project is entirely C# and built using dotnet core for cross platform capability. The project was built, run and tested predominately using Visual Studio IDE.
However, the project can be built and run from the dotnet CLI which is [provided by Microsoft](https://dotnet.microsoft.com/download)

## Experiment

The approach to experiment is to generate bit strings for fixed sample such as 1, 2, 3, ... 16 and varying the 4 input parameters of Angle Modulation to count how frequently any bit string is produced,
in an attempt to evaluate the efficiency of the technique and to identify potenital biases of the technique.

The parameter values are generated using constants and can be configured accordingly. Likewise, the sample set is also generated using constants and can be configured as needed.
