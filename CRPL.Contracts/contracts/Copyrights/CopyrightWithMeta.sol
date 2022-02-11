// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./CopyrightBase.sol";
import "../ICopyrightMeta.sol";
import "../Structs/Meta.sol";

abstract contract CopyrightWithMeta is CopyrightBase, ICopyrightMeta {

    mapping (uint256 => Meta) internal _metadata;

    constructor(string memory _name) CopyrightBase(_name) 
    {
    }

    function RegisterWithMeta(OwnershipStake[] memory to, Meta memory def) external 
    {
        uint256 rightId = super._register(to);
        _metadata[rightId] = def;
    }

    function Title(uint256 rightId) external override view returns (string memory) 
    {
        return _metadata[rightId].title;
    }

    function ExpiresOn(uint256 rightId) external override view returns (uint256) 
    {
        return _metadata[rightId].expires;
    }

    function RegisterTime(uint256 rightId) external override view returns (uint256) 
    {
        return _metadata[rightId].registered;
    }

    function WorkHash(uint256 rightId) external override view returns (string memory) 
    {
        return _metadata[rightId].workHash;
    }

    function WorkURI(uint256 rightId) external override view returns (string memory) 
    {
        return _metadata[rightId].workUri;
    }

    function WorkType(uint256 rightId) external override view returns (string memory)
    {
        return _metadata[rightId].workType;
    }

    function LegalMeta(uint256 rightId) external override view returns (string memory) 
    {
        return _metadata[rightId].legalMeta;
    }

    function CopyrightProtections(uint256 rightId) external override view returns (Protections memory) 
    {
        return _metadata[rightId].protections;
    }

    function CopyrightMeta(uint256 rightId) external override view returns (Meta memory)
    {
        return _metadata[rightId];
    }
}