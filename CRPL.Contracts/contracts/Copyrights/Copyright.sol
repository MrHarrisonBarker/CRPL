// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "../CopyrightBase.sol";
import "../Structs/CopyrightState.sol";
import "../Structs/OwnershipStructure.sol";
import "../Utils/IdCounters.sol";

abstract contract Copyright is CopyrightBase {
    using IdCounters for IdCounters.IdCounter;

    string internal _name;

    // rightId -> ownership structures
    mapping (uint256 => OwnershipStructure[]) internal _shareholders;

    // owner -> number of copyrights
    mapping (address => uint256) internal _numOfRights;

    // rightId -> new ownership
    mapping (uint256 => OwnershipStructure[]) internal _newHolders;

    // rightId -> shareholder -> bool (prop vote)
    mapping (uint256 => mapping (address => bool)) internal _proposalVotes;

    // rightId -> number of votes
    mapping (uint256 => uint8) internal _numOfPropVotes;

    // rightId -> approved address
    mapping (uint256 => address) internal _approvedAddress;

    // owner address -> (operator address -> approved)
    mapping (address => mapping (address => bool)) internal ownerToOperators;

    IdCounters.IdCounter internal _copyCount;

    constructor(string memory name) {
        _name = name;
    }

    function OwnershipOf(uint256 rightId) external override view returns (OwnershipStructure[] memory) {       
        return _shareholders[rightId];
    }

    function PortfolioSize(address owner) external override view returns (uint256) {
        return _numOfRights[owner];
    }

    function Register(OwnershipStructure[] memory to) public {

        uint256 rightId = _copyCount.next();

        // registering copyright across all shareholders
        for (uint8 i = 0; i < to.length; i++) {
            _recordRight(to[i].owner);
            _shareholders[rightId].push(to[i]);
        }

        emit Registered(rightId, to);
    }

    function ProposeRestructure(uint256 rightId, OwnershipStructure[] memory restructured) external override payable {
        
        for (uint8 i = 0; i < restructured.length; i++) {
            _newHolders[rightId].push(restructured[i]);
        }   

        emit ProposedRestructure(rightId, _getProposedRestructure(rightId));
    }

    function Proposal(uint256 rightId) external override view returns (RestructureProposal memory) {
        return _getProposedRestructure(rightId);
    }

    function BindRestructure(uint256 rightId, bool accepted) external override payable {

        // record vote
        _proposalVotes[rightId][msg.sender] = accepted;
        _numOfPropVotes[rightId] ++;

        // if the proposal has enough votes, **** 100% SHAREHOLDER CONSENSUS ****
        if (_numOfPropVotes[rightId] == _numberOfShareholder(rightId)) {
            
            // proposal has been accepted and is now binding

            // reset has to happen before new shareholders are registered to remove data concerning old shareholders
            _resetProposal(rightId);

            _shareholders[rightId] = _newHolders[rightId];

            delete(_newHolders[rightId]);

            emit Restructured(rightId, _getProposedRestructure(rightId));
        }

    }

    function ApproveOne(uint256 rightId, address approved) external override payable {
        // check approved is not owner of copyright

        _approvedAddress[rightId] = approved;

        emit Approved(rightId, approved);
    }

    function ApproveManager(address manager, bool hasApproval) external override {
        
        ownerToOperators[msg.sender][manager] = hasApproval;

        emit ApprovedManager(msg.sender, manager, hasApproval);
    }

    function GetApproved(uint256 rightId) external override view returns (address) {
        return _approvedAddress[rightId];
    }

    function IsManager(address client, address manager) external override view returns (bool) {
        return ownerToOperators[client][manager];
    }

    //////////// INTERNAL METHODS ////////////

    function _numberOfShareholder(uint256 rightId) internal view returns (uint256) {
        return _shareholders[rightId].length;
    }

    function _recordRight(address shareholder) internal {
        _numOfRights[shareholder] += 1;
    }

    function _getProposedRestructure(uint256 rightId) internal view returns(RestructureProposal memory) {
        return RestructureProposal({oldStructure: _shareholders[rightId], newStructure: _newHolders[rightId]});
    }

    function _resetProposal(uint256 rightId) internal {        

        OwnershipStructure[] memory holdersWhoVoted = _shareholders[rightId];

        for (uint8 i = 0; i < holdersWhoVoted.length; i++) {
            delete(_proposalVotes[rightId][holdersWhoVoted[i].owner]);
        }

        _numOfPropVotes[rightId] = 0;

    }

}