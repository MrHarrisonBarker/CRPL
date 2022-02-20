// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./CopyrightBase.sol";
import "../Utils/IdCounters.sol";

/// @dev Implementation of a traditional copyright contract using CopyrightBase
contract Copyright is CopyrightBase {
    using IdCounters for IdCounters.IdCounter;

    constructor() CopyrightBase("CRPL COPYRIGHT BACKED BY PIPO") payable {}
}