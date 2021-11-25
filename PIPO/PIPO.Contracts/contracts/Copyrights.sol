// // SPDX-License-Identifier: MIT
// pragma solidity >=0.4.22 <0.9.0;

// import 'openzeppelin-solidity/contracts/token/ERC721/ERC721.sol';
// import 'openzeppelin-solidity/contracts/access/Ownable.sol';
// import 'openzeppelin-solidity/contracts/utils/Counters.sol';

// contract Copyrights is ERC721, Ownable 
// {
//   using Counters for Counters.Counter;
//   Counters.Counter private _tokenIds;
  
//   constructor() ERC721("Standard Copyright", "CRPL") public {}
  
//   function simpleMint(address owner) external onlyOwner returns (uint256)
//   {
//     _tokenIds.increment();
    
//     uint256 nextTokenId = _tokenIds.current();
    
//     _mint(owner, nextTokenId);
    
//     return nextTokenId;
//   }
// }
