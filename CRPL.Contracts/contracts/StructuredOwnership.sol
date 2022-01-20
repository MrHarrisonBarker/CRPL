// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./OwnershipStructure.sol";

interface StructuredOwnership {
    
    /// @dev Emits when a new copyright is registered
    event Registered(uint256 indexed rightId, OwnershipStructure to);

    /// @param rightId The copyright id
    event Restructured(uint256 indexed rightId, bytes data);

    /// @notice The current ownership structure of a copyright
    /// @dev
    /// @param rightId The copyright id
    function Ownership(uint256 rightId) external view returns (OwnershipStructure[] memory);

        /// @notice Restructures the ownership share of a copyright contract, this change must be bound by all share holders
    /// @dev 
    /// @param restructured The new owernship shares
    /// @param notes Any notes written concerning restructure for public record
    /// @return The restructure id
    function Restructure(uint256 restructureId, OwnershipStructure[] memory restructured, bytes memory notes) external payable returns(uint256);

    /// @notice Binds a shareholders vote to a restructure
    /// @dev Must authorize shareholder
    /// @param rightId The copyright id
    /// @param accepted If the shareholder accepts the restructure
    function BindRestructure(uint256 rightId, bool accepted) external payable;
}