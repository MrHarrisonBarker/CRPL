// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "../ICopyright.sol";
import "../Structs/OwnershipStake.sol";
import "../Structs/ProposalVote.sol";
import "../Utils/IdCounters.sol";

abstract contract Copyright is ICopyright {
    using IdCounters for IdCounters.IdCounter;

    bool internal _locked;
    string internal _name;

    string constant INVALID_ADDR = "INVALID_ADDR";
    string constant NOT_SHAREHOLDER = "NOT_SHAREHOLDER";
    string constant THREAD_LOCKED = "THREAD_LOCKED";
    string constant NOT_ALLOWED = "NOT_ALLOWED";
    string constant NOT_APPROVED = "NOT_APPROVED";
    string constant NOT_VALID_RIGHT = "NOT_VALID_RIGHT";
    string constant INVALID_SHARE = "INVALID_SHARE";
    string constant NO_SHAREHOLDERS = "NO_SHAREHOLDERS";
    string constant ALREADY_VOTED = "ALREADY_VOTED";

    // rightId -> ownership structures
    mapping (uint256 => OwnershipStake[]) internal _shareholders;

    // owner -> number of copyrights
    mapping (address => uint256) internal _numOfRights;

    // rightId -> new ownership
    mapping (uint256 => OwnershipStake[]) internal _newHolders;

    // rightId -> shareholder -> bool (prop vote)
    mapping (uint256 => ProposalVote[]) internal _proposalVotes;

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

    function OwnershipOf(uint256 rightId) external override validId(rightId) view returns (OwnershipStake[] memory) {       
        return _shareholders[rightId];
    }

    function PortfolioSize(address owner) external override validAddress(owner) view returns (uint256) {
        return _numOfRights[owner];
    }

    function Register(OwnershipStake[] memory to) public validShareholders(to) {

        uint256 rightId = _copyCount.next();

        // registering copyright across all shareholders
        for (uint8 i = 0; i < to.length; i++) {

            require(to[i].share > 0, INVALID_SHARE);

            _recordRight(to[i].owner);
            _shareholders[rightId].push(to[i]);
        }

        emit Registered(rightId, to);
    }

    function _register (OwnershipStake[] memory to) internal validShareholders(to) returns (uint256) {
        Register(to);
        return _copyCount.getCurrent();
    }

    function ProposeRestructure(uint256 rightId, OwnershipStake[] memory restructured) external override validId(rightId) validShareholders(restructured) isShareholder(rightId, msg.sender) payable {
        
        for (uint8 i = 0; i < restructured.length; i++) {

            require(restructured[i].share > 0, INVALID_SHARE);

            _newHolders[rightId].push(restructured[i]);
        }   

        emit ProposedRestructure(rightId, _getProposedRestructure(rightId));
    }

    function Proposal(uint256 rightId) external override validId(rightId) view returns (RestructureProposal memory) {
        return _getProposedRestructure(rightId);
    }

    function BindRestructure(uint256 rightId, bool accepted) external override isShareholder(rightId, msg.sender) validId(rightId) payable 
    {
        _checkHasVoted(rightId, msg.sender);
     
        // record vote
        _proposalVotes[rightId].push(ProposalVote(msg.sender, accepted));
        _numOfPropVotes[rightId] ++;

        for (uint8 i = 0; i < _proposalVotes[rightId].length; i ++) 
        {
            if (!_proposalVotes[rightId][i].accepted) 
            {
                _resetProposal(rightId);
                emit FailedProposal(rightId);

                return;
            }
        }

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

    function ApproveOne(uint256 rightId, address approved) external override validId(rightId) validAddress(approved) isShareholder(rightId, msg.sender) payable {
        // check approved is not owner of copyright

        _approvedAddress[rightId] = approved;

        emit Approved(rightId, approved);
    }

    function ApproveManager(address manager, bool hasApproval) external override validAddress(manager) {
        
        ownerToOperators[msg.sender][manager] = hasApproval;

        emit ApprovedManager(msg.sender, manager, hasApproval);
    }

    function GetApproved(uint256 rightId) external override validId(rightId) view returns (address) {
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

        OwnershipStake[] memory holdersWhoVoted = _shareholders[rightId];

        for (uint8 i = 0; i < holdersWhoVoted.length; i++) {
            delete(_proposalVotes[rightId]);
        }

        _numOfPropVotes[rightId] = 0;

    }

    function _checkHasVoted(uint256 rightId, address addr) internal view
    {
        for (uint8 i = 0; i < _proposalVotes[rightId].length; i ++)
        {
            require(_proposalVotes[rightId][i].voter != addr, ALREADY_VOTED);
        }
    }

    //////////// MODIFIERS ////////////

    modifier isShareholder(uint256 rightId, address addr) 
    {
        uint8 c = 0;
        for (uint8 i = 0; i < _shareholders[rightId].length; i++) 
        {
            if (_shareholders[rightId][i].owner == addr) c ++;
        }
        require(c == 1, NOT_SHAREHOLDER);
        _;
    }

    modifier validAddress(address addr)
    {
        require(addr != address(0), INVALID_ADDR);
        _;
    }

    // TODO: could cause a problem
    modifier validId(uint256 rightId)
    {
        require(_shareholders[rightId].length == 0 || _shareholders[rightId][0].owner != address(0), NOT_VALID_RIGHT);
        _;
    }

    modifier validShareholders(OwnershipStake[] memory holders) 
    {
        require(holders.length > 0, NO_SHAREHOLDERS);

        for (uint8 i = 0; i < holders.length; i ++) 
        {
            require(holders[i].owner != address(0), INVALID_ADDR);
        }
        _;
    }

    modifier atomicLocked() 
    {
        require(!_locked, THREAD_LOCKED);

        _locked = true;
        _;
        _locked = false;
    }

    modifier isApproved(uint256 rightId, address addr)
    {
        require(_approvedAddress[rightId] == addr, NOT_APPROVED);
        _;
    }

}