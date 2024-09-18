namespace TKC.Models.PlanningCenter
{
    public class GroupsResponse
    {
        public List<Group> data { get; set; } = new List<Group>();
        public Meta meta { get; set; } = new Meta();
        public List<EnrollmentData> included { get; set; } = new();
    }

    public class Meta
    {
        public bool onboarding { get; set; }
        public int total_count { get; set; }
        public int count { get; set; }
    }
    public class Group
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Links links { get; set; }
    }

    public class GroupWithEnrollment
    {
        public Group group { get; set; }
        public EnrollmentAttributes enrollment { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
        public string html { get; set; }

    }

    public class Attributes
    {
        public DateTime? archived_at { get; set; }
        public string contact_email { get; set; } = "";
        public DateTime created_at { get; set; }

        public string description { get; set; } = ""; //html
        public string events_visibility { get; set; } = ""; //public
        public HeaderImage? header_image { get; set; }
        public bool leaders_can_search_people_database { get; set; }
        public string location_type_preference { get; set; } = ""; //physical
        public int memberships_count { get; set; }
        public string name { get; set; } = "";
        public string public_church_center_web_url { get; set; } = ""; // url to it
        public string schedule { get; set; } = ""; //when description
        public string? virtual_location_url { get; set; }

    }

    public class HeaderImage
    {
        public string thumbnail { get; set; } = "";
        public string medium { get; set; } = "";
        public string original { get; set; } = "";
    }

    // ENROLLMENT

    public class EnrollmentResponse
    {
        public EnrollmentData data { get; set; }
    }

    public class EnrollmentData
    {
        public string type { get; set; }
        public string id { get; set; } // id of group
        public EnrollmentAttributes attributes { get; set; }
    }

    public class EnrollmentAttributes
    {
        public bool auto_closed { get; set; }
        public object auto_closed_reason { get; set; }
        public object date_limit { get; set; }
        public bool date_limit_reached { get; set; }
        public object member_limit { get; set; }
        public bool member_limit_reached { get; set; }
        public string status { get; set; } //private, closed
        public string strategy { get; set; } //request_to_join, closed
    }

}