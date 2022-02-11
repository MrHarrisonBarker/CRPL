// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

struct Protections {
    bool authorship;

    bool commercialAdaptation;
    bool nonCommercialAdaptation;
    
    bool reviewOrCrit;

    bool commercialPerformance;
    bool nonCommercialPerformance;

    bool commercialReproduction;
    bool nonCommercialReproduction;
    
    bool commercialDistribution;
    bool nonCommercialDistribution;
}