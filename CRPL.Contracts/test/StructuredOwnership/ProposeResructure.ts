import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import {Copyright} from "../../typechain";

describe("ProposeRestructure", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Copyright;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;
    let address2: SignerWithAddress;
    let addresses: SignerWithAddress[];

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Copyright");

        [owner, address1, address2, ...addresses] = await ethers.getSigners();
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 70},
            {owner: address1.address, share: 30}
        ];

        let meta = {
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

        deployedContract = await contractFactory.deploy() as Copyright;

        await deployedContract.deployed();
        await deployedContract.Register(ownershipStructure, meta);
    });

    it('Should propose new structure', async function ()
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ]

        let res = await deployedContract.ProposeRestructure(1, ownershipStructure);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("ProposedRestructure")
        });

        expect(await deployedContract.Proposal(1)).to.eql([
            [
                [owner.address, 70],
                [address1.address, 30]
            ],
            [
                [owner.address, 50],
                [address1.address, 50]
            ]
        ]);
    });

    it('Should REVERT when not valid rightId', async function ()
    {
        await expect(deployedContract.ProposeRestructure(2, [])).to.be.revertedWith('NOT_VALID_RIGHT');
    });

    it('Should REVERT no shareholders', async function ()
    {
        await expect(deployedContract.ProposeRestructure(1, [])).to.be.revertedWith('NO_SHAREHOLDERS');
    });

    it('Should REVERT when invalid shareholders', async function ()
    {
        await expect(deployedContract.ProposeRestructure(1, [{owner: ethers.constants.AddressZero, share: 1}])).to.be.revertedWith('INVALID_ADDR');
    });

    it('Should REVERT when not sent from shareholder or approved', async function ()
    {
        await expect(deployedContract.connect(address2).ProposeRestructure(1, [{owner: owner.address, share: 1}])).to.be.revertedWith('NOT_SHAREHOLDER');
    });

});