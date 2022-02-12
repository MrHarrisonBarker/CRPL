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

    function CopyrightMeta(uint256 rightId) external override view returns (Meta memory)
    {
        return _metadata[rightId];
    }
}