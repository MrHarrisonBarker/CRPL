// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./OwnershipStake.sol";

struct RestructureProposal {
    OwnershipStake[] oldStructure;
    OwnershipStake[] newStructure;
}
