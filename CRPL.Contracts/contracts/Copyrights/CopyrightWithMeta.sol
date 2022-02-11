// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Copyright.sol";
import "../ICopyrightMeta.sol";
import "../Structs/Meta.sol";

abstract contract CopyrightWithMeta is Copyright, ICopyrightMeta {

    string internal _legalDefinition;

    mapping (uint256 => Meta) _metadata;

    constructor(string memory name, string memory legalDef) Copyright(name) 
    {
        _legalDefinition = legalDef;
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

    function LegalMeta(uint256 rightId) external override view returns (string memory) 
    {
        return _metadata[rightId].legalMeta;
    }

    function LegalDefinition() external override view returns (string memory) 
    {
        return _legalDefinition;
    }

    function CopyrightMeta(uint256 rightId) external override view returns (Meta memory)
    {
        return _metadata[rightId];
    }
}