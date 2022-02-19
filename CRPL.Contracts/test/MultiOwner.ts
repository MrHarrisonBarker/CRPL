import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../typechain";


describe("MultiOwner", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Copyright;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;
    let address2: SignerWithAddress;
    let addresses: SignerWithAddress[];
    let ownershipStructure: { owner: string; share: BigNumberish }[];

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Copyright");

        [owner, address1, address2, ...addresses] = await ethers.getSigners();
        ownershipStructure = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ];
        
        for (const address of addresses) {
            console.log(address.address);
            ownershipStructure.push({owner: address.address, share: 10});
        }

        deployedContract = await contractFactory.deploy() as Copyright;

        await deployedContract.deployed();
        await deployedContract.Register(ownershipStructure);
    });

    it('Should have ownership structure with many addresses', async function ()
    {
        let ownership = await deployedContract.OwnershipOf(1);

        let ownershipArray = Object.keys(ownership).map(function(index){
            // @ts-ignore
            let o = ownership[index];
            return [o.owner, o.share];
        });

        expect(ownership).to.eql(ownershipArray);
    });

});