namespace CRPL.Data.Applications.ViewModels;

public static class ApplicationMapper
{
    public static ApplicationViewModel Map(this Application application)
    {
        ApplicationViewModel viewModel = null;

        switch (application.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                viewModel = new CopyrightRegistrationViewModel();
                break;
            case ApplicationType.OwnershipRestructure:
                break;
            case ApplicationType.CopyrightTypeChange:
                break;
            case ApplicationType.Dispute:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (viewModel == null)
        {
            throw new Exception();
        }

        if (application?.Fields == null)
        {
            throw new Exception();
        }

        foreach (var field in application.Fields)
        {
            var property = viewModel.GetType().GetProperty(field.Field);
            if (property != null) property.SetValue(viewModel, Convert.ChangeType(field.Value, property.PropertyType));
        }

        viewModel.Created = application.Created;
        viewModel.Modified = application.Modified;
        viewModel.Id = application.Id;

        return viewModel;
    }
}