// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

// Influenced by EIP-712, see: https://eips.ethereum.org/EIPS/eip-721

import "./IStructuredOwnership.sol";

/// @title Basic structure for interfacing with a copyright contract
interface ICopyright is IStructuredOwnership {

    /// @dev Emits when a copyright dispute has been registered
    event Disputed(uint256 indexed rightId, address indexed by, bytes reason);

    /// @dev Emits when a new address is approved to a copyright
    event Approved(uint256 indexed rightId, address indexed approved);

    /// @dev Emits when a new manager has been approved
    event ApprovedManager(address indexed owner, address indexed manager, bool hasApproval);

    /// @dev Emits after any modification
    event Modify(uint256 indexed rightId, bytes modification);

    // @notice gets all the rights held by address
    /// @param owner portfolios owner address
    // function Portfolio(address owner) external view returns (uint256[] memory);

    // @notice gets the number of rights held by an address
    /// @param owner portfolios owner address
    function PortfolioSize(address owner) external view returns (uint256);

    /// @notice Approve address for the copyright
    /// @dev Must authorize shareholder
    /// @param approved Address to be approved
    /// @param rightId The copyright id
    function ApproveOne(uint256 rightId, address approved) external payable;

    /// @notice Approve address to be manager of a users whole portfolio
    /// @dev Must authorize shareholder
    /// @param manager Address of the new manager
    /// @param hasApproval If the address has authority
    function ApproveManager(address manager, bool hasApproval) external;

    /// @notice Gets the approved address for a copyright
    /// @dev
    /// @param rightId The copyright id
    /// @return The approved address
    function GetApproved(uint256 rightId) external view returns (address);

    /// @notice Asks if address is a manager of a user/clients portfolio
    /// @dev
    /// @param client The address of the client in question
    /// @param manager The address of manager to be checked
    /// @return If the specific address (manager) has authority for client
    function IsManager(address client, address manager) external view returns (bool);

    //TODO: Dispute filing, cancelling and resolving
} 

//TODO: Implement EIP-165, see: https://eips.ethereum.org/EIPS/eip-165