import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../../typechain";


describe("Meta", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Copyright;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;
    let address2: SignerWithAddress;
    let addresses: SignerWithAddress[];

    let meta: { title: string; expires: BigNumberish; registered: BigNumberish; workHash: string; workUri: string; legalMeta: string; workType: string; protections: { authorship: boolean; commercialAdaptation: boolean; nonCommercialAdaptation: boolean; reviewOrCrit: boolean; commercialPerformance: boolean; nonCommercialPerformance: boolean; commercialReproduction: boolean; nonCommercialReproduction: boolean; commercialDistribution: boolean; nonCommercialDistribution: boolean } };

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Copyright");

        [owner, address1, address2, ...addresses] = await ethers.getSigners();

        deployedContract = await contractFactory.deploy() as Copyright;

        await deployedContract.deployed();
    });

    it('Should register with metadata', async function ()
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 100}
        ];
        
        meta = {
            title: "Hello world",
            expires: Date.now() + 999999,
            registered: 0,
            workHash: "",
            workUri: "",
            legalMeta: "",
            workType: "",
            protections: {
                authorship: false,
                commercialAdaptation: false,
                nonCommercialAdaptation: false,
                reviewOrCrit: false,
                commercialPerformance: false,
                nonCommercialPerformance: false,
                commercialReproduction: false,
                nonCommercialReproduction: false,
                commercialDistribution: false,
                nonCommercialDistribution: false
            }
        };
        
        let res = await deployedContract.Register(ownershipStructure, meta);

        res.wait().then(value =>
        {
            expect(value.events != null && value.events[0].event).to.equal("Registered")
        });
    });

    it('Should get registered metadata', async function ()
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 100}
        ];

        await deployedContract.Register(ownershipStructure, meta);
        
        let res = await deployedContract.CopyrightMeta(1);
        
        expect(res.title).to.eql(meta.title);
    });

    it('Should REVERT no shareholders', async function ()
    {
        await expect(deployedContract.Register([], meta)).to.be.revertedWith('NO_SHAREHOLDERS');
    });

    it('Should REVERT when invalid shareholders', async function ()
    {
        await expect(deployedContract.Register([{owner: ethers.constants.AddressZero, share: 1}], meta)).to.be.revertedWith('INVALID_ADDR');
    });
    
});