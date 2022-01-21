// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

library IdCounters {

    struct IdCounter {
        uint256 _count;
    }

    function getCurrent(IdCounter storage count) internal view returns (uint256) {
        return count._count;
    }

    function inc(IdCounter storage count) internal {
        count._count += 1;
    }

    function next(IdCounter storage count) internal returns (uint256) {
        inc(count);
        return count._count;
    }

}