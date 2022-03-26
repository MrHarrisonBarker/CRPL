// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Structs/OwnershipStake.sol";
import "./Structs/RestructureProposal.sol";
import "./Structs/ProposalVote.sol";

/// @title Definition of a multi party shareholder style ownership structure, with consensus voting
interface IStructuredOwnership {
    
    /// @dev Emits when a new copyright is registered
    event Registered(uint256 indexed rightId, OwnershipStake[] to);

    /// @dev Emits when a copyright has been restructured and bound
    event Restructured(uint256 indexed rightId, RestructureProposal proposal);

    /// @dev Emits when a restructure is proposed
    event ProposedRestructure(uint256 indexed rightId, RestructureProposal proposal);

    /// @dev Emits when a restructure vote fails
    event FailedProposal(uint256 indexed rightId);

    /// @notice The current ownership structure of a copyright
    /// @param rightId The copyright id
    function OwnershipOf(uint256 rightId) external view returns (OwnershipStake[] memory);

    /// @notice Proposes a restructure of the ownership share of a copyright contract, this change must be bound by all share holders
    /// @param rightId The copyright id
    /// @param restructured The new ownership shares
    //  @param notes Any notes written concerning restructure for public record
    function ProposeRestructure(uint256 rightId, OwnershipStake[] memory restructured) external payable;

    /// @notice The current restructure proposal for a copyright
    /// @param rightId The copyright id
    /// @return A restructure proposal
    function Proposal(uint256 rightId) external view returns (RestructureProposal memory);
    
    function CurrentVotes(uint256 rightId) external view returns (ProposalVote[] memory);

    /// @notice Binds a shareholders vote to a restructure
    /// @param rightId The copyright id
    /// @param accepted If the shareholder accepts the restructure
    function BindRestructure(uint256 rightId, bool accepted) external payable;
}