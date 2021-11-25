// SPDX-License-Identifier: MIT
pragma solidity >=0.4.22 <0.9.0;

abstract contract SimpleOwnable
{
  address private _owner;
  
  // sets the owner to the caller
  constructor()
  {
    setOwner(msg.sender);
  }
  
  // returns the current owner
  function owner() public view returns(address) 
  {
    return _owner;
  }
  
  // A modifier to check if the caller owns the contract
  modifier owns() 
  {
    require(msg.sender == _owner, "You are not the owner!");
    _;
  }
  
  // takes an new address as a parameter and sets the owner variable to the new address
  function setOwner(address newAddress) private 
  {
    _owner = newAddress;
  }
  
  // transfers the ownership to a new address
  function transferOwner(address to) public owns
  {
    emit ChangeOfOwnership(_owner, to);
    setOwner(to);
  } 
  
  event ChangeOfOwnership(address from, address to);
  
}

contract SimpleOwnership is SimpleOwnable
{
  uint256 private _numberOfOwnerships;
  
  string private _name;
  
  constructor(string memory name) {
    _name = name;
  }
  
  function numberOfOwnerships() public view returns (uint256) {
    return _numberOfOwnerships;
  }
  
  function simpleMint(address receiver) public
  {
    transferOwner(receiver);
    
    _numberOfOwnerships++;
  }
}
