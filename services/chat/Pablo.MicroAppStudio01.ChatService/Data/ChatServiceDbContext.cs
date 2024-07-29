using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Chat.Conversations;
using Volo.Chat.EntityFrameworkCore;
using Volo.Chat.Messages;
using Volo.Chat.Users;

namespace Pablo.MicroAppStudio01.ChatService.Data;

[ConnectionStringName(DatabaseName)]
[ReplaceDbContext(typeof(IChatDbContext))]
public class ChatServiceDbContext :
    AbpDbContext<ChatServiceDbContext>,
    IChatDbContext,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DbTablePrefix = "";
    public const string DbSchema = null;
    
    public const string DatabaseName = "ChatService";
    
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public DbSet<Message> ChatMessages { get; set; }
    public DbSet<ChatUser> ChatUsers { get;set;  }
    public DbSet<UserMessage> ChatUserMessages { get; set; }
    public DbSet<Conversation> ChatConversations { get; set; }

    public ChatServiceDbContext(DbContextOptions<ChatServiceDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
        
        builder.ConfigureChat();
    }
}