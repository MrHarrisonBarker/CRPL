import { expect } from "chai";
// @ts-ignore
import { ethers } from "hardhat";

describe("Copyleft", function () {
    it("Should construct copyleft copyright contract with legal def", async function () {
        const CopyleftCopyright = await ethers.getContractFactory("Copyleft");

        const deployedContract = await CopyleftCopyright.deploy();
        await deployedContract.deployed();

        expect(await deployedContract.LegalDefinition()).to.equal("https://www.gnu.org/licenses/gpl-3.0.html");
    });
});