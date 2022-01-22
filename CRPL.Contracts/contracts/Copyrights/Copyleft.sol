// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./CopyrightWithMeta.sol";
import "../Utils/IdCounters.sol";

/// @dev Implementation of a "copyleft" contract using CopyrightBase
contract Copyleft is CopyrightWithMeta {
    using IdCounters for IdCounters.IdCounter;

    // What is copyleft?
    // 1. giving the right to distribute and modify a work
    // Freedom 0
    // the freedom to use the work
    // Freedom 1
    // the freedom to study the work
    // Freedom 2
    // the freedom to copy and share the work with others
    // Freedom 3
    // the freedom to modify the work, and the freedom to distribute modified and therefore derivative works

    constructor() CopyrightWithMeta("Copyleft", "s") payable {
        // super("Copyleft");
    }
}