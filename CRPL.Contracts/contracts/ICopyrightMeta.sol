// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Structs/Meta.sol";
import "./Structs/Protections.sol";

/// @title Copyright meta and legal data
interface ICopyrightMeta {
    /// @notice all metadata about copyright
    function CopyrightMeta(uint256 rightId) external view returns (Meta memory);
}