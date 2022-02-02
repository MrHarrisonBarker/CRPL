import { expect } from "chai";
// @ts-ignore
import { ethers } from "hardhat";

describe("Permissive", function () {
    it("Should construct permissive copyright contract with legal def", async function () {
        const PermissiveCopyright = await ethers.getContractFactory("Permissive");

        const deployedContract = await PermissiveCopyright.deploy();
        await deployedContract.deployed();

        expect(await deployedContract.LegalDefinition()).to.equal("https://creativecommons.org/licenses/by/2.0");
    });
});