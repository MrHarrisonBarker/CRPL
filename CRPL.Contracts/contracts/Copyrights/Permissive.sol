// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./CopyrightWithMeta.sol";
import "../Utils/IdCounters.sol";

/// @dev Implementation of a traditional copyright contract using CopyrightBase
contract Permissive is CopyrightWithMeta {
    using IdCounters for IdCounters.IdCounter;

    constructor() CopyrightWithMeta("Standard Copyright", "https://creativecommons.org/licenses/by/2.0") payable {}
}