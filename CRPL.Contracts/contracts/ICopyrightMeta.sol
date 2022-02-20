// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Structs/Meta.sol";
import "./Structs/Protections.sol";
import "./ICopyright.sol";

/// @title Copyright meta and legal data
interface ICopyrightMeta is ICopyright {
    /// @notice all metadata about copyright
    function CopyrightMeta(uint256 rightId) external view returns (Meta memory);
}