// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "../CopyrightBase.sol";
import "../CopyrightState.sol";
import "../OwnershipStructure.sol";
import "../Utils/IdCounters.sol";

abstract contract Copyright is CopyrightBase {
    using IdCounters for IdCounters.IdCounter;

    string internal _name;

    // rightId -> ownership structures
    mapping (uint256 => OwnershipStructure[]) internal _shareholders;

    // owner -> number of copyrights
    mapping (address => uint256) internal _numOfRights;

    // rightId -> proposal
    // mapping (uint256 => RestructureProposal) internal _restructures;

    // rightId -> new ownership
    mapping (uint256 => OwnershipStructure[]) internal _newHolders;

    // rightId -> shareholder -> bool
    mapping (uint256 => mapping (address => bool)) internal _proposalVotes;

    // rightId -> number of votes
    mapping (uint256 => uint8) internal _numOfPropVotes;

    IdCounters.IdCounter internal _copyCount;

    constructor(string memory name) {
        _name = name;
        
    }

    function _numberOfShareholder(uint256 rightId) internal view returns (uint256) {
        return _shareholders[rightId].length;
    }

    function OwnershipOf(uint256 rightId) external override view returns (OwnershipStructure[] memory) {       
        return _shareholders[rightId];
    }

    // function Portfolio(address owner) external override view returns (uint256[] memory) {
    //     return _portfolios[owner];
    // }

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

    function _recordRight(address shareholder) internal {
        _numOfRights[shareholder] += 1;
    }

    function _getProposedRestructure(uint256 rightId) internal view returns(RestructureProposal memory) {
        return RestructureProposal({oldStructure: _shareholders[rightId], newStructure: _newHolders[rightId]});
    }

    function ProposeRestructure(uint256 rightId, OwnershipStructure[] memory restructured) external override payable {

        // RestructureProposal memory prop = RestructureProposal({oldStructure: OwnershipStructure[], newStructure: OwnershipStructure[]});

        // _restructures[rightId] = prop;
        
        for (uint8 i = 0; i < restructured.length; i++) {
            _newHolders[rightId].push(restructured[i]);
        }   

        emit ProposedRestructure(rightId, _getProposedRestructure(rightId));
    }

    function Proposal(uint256 rightId) external override view returns (RestructureProposal memory) {
        return _getProposedRestructure(rightId);
    }

    function _resetProposal(uint256 rightId) internal {
        delete(_newHolders[rightId]);

        OwnershipStructure[] memory holdersWhoVoted = _shareholders[rightId];

        for (uint8 i = 0; i < holdersWhoVoted.length; i++) {
            delete(_proposalVotes[rightId][holdersWhoVoted[i].owner]);
        }

        _numOfPropVotes[rightId] = 0;

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

            emit Restructured(rightId, _getProposedRestructure(rightId));
        }

    }

}