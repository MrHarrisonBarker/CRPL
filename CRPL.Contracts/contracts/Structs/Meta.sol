// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "./Protections.sol";

struct Meta {
    string title;
    uint256 expires;
    uint256 registered;
    string workHash;
    string workUri;
    string legalMeta;
    string workType;
    Protections protections;
}