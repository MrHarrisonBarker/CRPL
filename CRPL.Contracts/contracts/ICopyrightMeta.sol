// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Structs/Meta.sol";
import "./Structs/Protections.sol";

/// @title Copyright meta and legal data
interface ICopyrightMeta {

    /// @notice title of work
    function Title(uint256 rightId) external view returns (string memory);

    /// @notice when the copyright expires
    function ExpiresOn(uint256 rightId) external view returns (uint256);

    /// @notice time of registration of a work
    function RegisterTime(uint256 rightId) external view returns (uint256);

    /// @notice hash of the copyrighted work
    function WorkHash(uint256 rightId) external view returns (string memory);

    /// @notice URI for the copyrighted work
    function WorkURI(uint256 rightId) external view returns (string memory);

    /// @notice type of work copyrighted
    function WorkType(uint256 rightId) external view returns (string memory);

    /// @notice custom legal meta from the user
    function LegalMeta(uint256 rightId) external view returns (string memory);

    /// @notice legal protections afforded by contract
    function CopyrightProtections(uint256 rightId) external view returns (Protections memory);
    
    /// @notice all metadata about copyright
    function CopyrightMeta(uint256 rightId) external view returns (Meta memory);
}